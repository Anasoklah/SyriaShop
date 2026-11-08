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

        _ = await userManager.AddToRoleAsync(user, IdentitySeeder.CustomerRole);

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

        var newPair = await IssueTokensAsync(user, cancellationToken);

        var revoked = await refreshTokenRepository.TryRevokeActiveTokenAsync(
            stored.Token, now, newPair.RefreshToken, cancellationToken);

        if (!revoked)
        {
            // The token was concurrently re-used: treat as suspected theft.
            // Revoke every active token for this user and reject the request
            // without issuing the pending replacement token.
            await refreshTokenRepository.RevokeActiveTokensForUserAsync(stored.UserId, now, cancellationToken);

            throw new UnauthorizedException(InvalidRefreshTokenMessage);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return newPair;
    }

    public async Task RevokeUserTokensAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await refreshTokenRepository.RevokeActiveTokensForUserAsync(userId, timeProvider.GetUtcNow(), cancellationToken);

    private async Task<TokenPairResponse> IssueTokensAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var roles = await userManager.GetRolesAsync(user);
        var now = timeProvider.GetUtcNow();

        var (accessToken, accessExpiresAt) = accessTokenGenerator.Generate(
            user.Id,
            user.Email!,
            roles.ToArray(),
            configuration.GetSection("Auth:Issuer").Value!,
            configuration.GetSection("Auth:Audience").Value!,
            configuration.GetSection("Auth:SigningKey").Value!,
            AccessTokenLifetime,
            now);

        var refreshToken = RefreshToken.Issue(user.Id, Guid.NewGuid().ToString("N"), now.Add(RefreshTokenLifetime), now);

        refreshTokenRepository.Add(refreshToken);

        return new TokenPairResponse(accessToken, refreshToken.Token, accessExpiresAt);
    }
}
