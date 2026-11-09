using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SyriaShop.Application;
using SyriaShop.Application.Exceptions;
using SyriaShop.Application.Identity;
using SyriaShop.Domain;

namespace SyriaShop.Infrastructure.Identity;

public sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    AccessTokenGenerator accessTokenGenerator,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IConfiguration configuration,
    TimeProvider timeProvider) : IIdentityService
{
    private static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);
    private const string InvalidRefreshTokenMessage = "Invalid refresh token.";

    public async Task<TokenPairResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser { UserName = request.Email, Email = request.Email };

        var creation = await userManager.CreateAsync(user, request.Password);

        if (!creation.Succeeded)
        {
            throw new DomainException(string.Join(' ', creation.Errors.Select(e => $"{e.Code}: {e.Description}")));
        }

        var roleAssignment = await userManager.AddToRoleAsync(user, IdentitySeeder.CustomerRole);

        if (!roleAssignment.Succeeded)
        {
            throw new DomainException(string.Join(' ', roleAssignment.Errors.Select(e => $"{e.Code}: {e.Description}")));
        }

        var pair = await IssueTokensAsync(user, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return pair;
    }

    public async Task<TokenPairResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        var passwordValid = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

        if (!passwordValid.Succeeded)
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        var pair = await IssueTokensAsync(user, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return pair;
    }

    public async Task<TokenPairResponse> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken = default)
    {
        var stored = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken)
            ?? throw new UnauthorizedException(InvalidRefreshTokenMessage);

        var now = timeProvider.GetUtcNow();

        if (stored.IsExpired(now))
        {
            throw new UnauthorizedException(InvalidRefreshTokenMessage);
        }

        var user = await userManager.FindByIdAsync(stored.UserId.ToString())
            ?? throw new UnauthorizedException(InvalidRefreshTokenMessage);

        var (accessToken, accessTokenExpiresAt) = await CreateAccessTokenAsync(user, now);
        var nextRefreshToken = RefreshToken.Issue(user.Id, Guid.NewGuid().ToString("N"), now.Add(RefreshTokenLifetime), now);

        // The conditional revoke, the replacement insert and the theft
        // response revoke all commit as one unit of work; a partial state can
        // only survive a process crash between commands, never a failed save.
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        var revoked = await refreshTokenRepository.TryRevokeActiveTokenAsync(
            stored.Token, now, nextRefreshToken.Token, cancellationToken);

        if (revoked)
        {
            refreshTokenRepository.Add(nextRefreshToken);
        }
        else
        {
            // Reuse of an already-revoked token is treated as suspected theft:
            // instead of issuing the replacement token, every active token for
            // this user is revoked. Reuse of a rotated token also revokes the
            // whole family, including tokens issued to other devices - that is
            // the deliberate cost of fail-closed session security here.
            await refreshTokenRepository.RevokeActiveTokensForUserAsync(stored.UserId, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        if (!revoked)
        {
            throw new UnauthorizedException(InvalidRefreshTokenMessage);
        }

        return new TokenPairResponse(accessToken, nextRefreshToken.Token, accessTokenExpiresAt);
    }

    public async Task RevokeUserTokensAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await refreshTokenRepository.RevokeActiveTokensForUserAsync(userId, cancellationToken);

    private async Task<(string AccessToken, DateTimeOffset ExpiresAt)> CreateAccessTokenAsync(
        ApplicationUser user,
        DateTimeOffset now)
    {
        var roles = await userManager.GetRolesAsync(user);

        var (accessToken, expiresAt) = accessTokenGenerator.Generate(
            user.Id,
            user.Email!,
            roles.ToArray(),
            configuration.GetSection("Auth:Issuer").Value!,
            configuration.GetSection("Auth:Audience").Value!,
            configuration.GetSection("Auth:SigningKey").Value!,
            AccessTokenLifetime,
            now);

        return (accessToken, expiresAt);
    }

    private async Task<TokenPairResponse> IssueTokensAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var (accessToken, accessExpiresAt) = await CreateAccessTokenAsync(user, now);

        var refreshToken = RefreshToken.Issue(user.Id, Guid.NewGuid().ToString("N"), now.Add(RefreshTokenLifetime), now);

        refreshTokenRepository.Add(refreshToken);

        return new TokenPairResponse(accessToken, refreshToken.Token, accessExpiresAt);
    }
}
