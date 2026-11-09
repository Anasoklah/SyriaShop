using SyriaShop.Application.Identity;

namespace SyriaShop.Application.Identity;

public interface IIdentityService
{
    Task<TokenPairResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<TokenPairResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<TokenPairResponse> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken = default);

    Task RevokeUserTokensAsync(Guid userId, CancellationToken cancellationToken = default);
}
