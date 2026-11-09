namespace SyriaShop.Domain;

public sealed class RefreshToken : Entity
{
    private RefreshToken()
    {
    }

    private RefreshToken(Guid userId, string token, DateTimeOffset expiresAt)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
    }

    public Guid UserId { get; private set; }

    public string Token { get; private set; } = null!;

    public DateTimeOffset ExpiresAt { get; private set; }

    public DateTimeOffset? RevokedAt { get; private set; }

    public string? ReplacedByToken { get; private set; }

    public bool IsExpired(DateTimeOffset now) => ExpiresAt <= now;

    public static RefreshToken Issue(Guid userId, string token, DateTimeOffset expiresAt, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new DomainException("Refresh token value must not be empty.");
        }

        if (expiresAt <= now)
        {
            throw new DomainException("Refresh token must expire in the future.");
        }

        return new RefreshToken(userId, token, expiresAt);
    }
}
