namespace SyriaShop.Application.Identity;

public sealed record RegisterRequest(string Email, string Password);

public sealed record LoginRequest(string Email, string Password);

public sealed record RefreshRequest(string RefreshToken);

public sealed record TokenPairResponse(string AccessToken, string RefreshToken, DateTimeOffset AccessTokenExpiresAtUtc);
