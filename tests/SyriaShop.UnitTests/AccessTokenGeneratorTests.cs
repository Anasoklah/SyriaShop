using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SyriaShop.Infrastructure.Identity;

namespace SyriaShop.UnitTests;

public class AccessTokenGeneratorTests
{
    private const string SigningKey = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";
    private static readonly DateTimeOffset Now = new(2026, 2, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(15);

    [Fact]
    public void Generate_ContainsUserEmailAndRoleClaims()
    {
        var generator = new AccessTokenGenerator();
        var userId = Guid.NewGuid();
        var roles = new[] { "Admin", "Customer" };

        var (token, expiresAt) = generator.Generate(userId, "admin@syriashop.dev", roles, "SyriaShop", "SyriaShopApi", SigningKey, Lifetime, Now);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        Assert.Equal(userId.ToString(), jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal("admin@syriashop.dev", jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.Equal(2, jwt.Claims.Count(c => c.Type == ClaimTypes.Role));
        Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
    }

    [Fact]
    public void Generate_ExpiryHonorsLifetime()
    {
        var generator = new AccessTokenGenerator();

        var (_, expiresAt) = generator.Generate(Guid.NewGuid(), "x@y.z", [], "issuer", "audience", SigningKey, Lifetime, Now);

        Assert.Equal(Now.Add(Lifetime), expiresAt);
    }

    [Fact]
    public void Generate_ProducesValidJwtWithThreeSegments()
    {
        var generator = new AccessTokenGenerator();

        var (token, _) = generator.Generate(Guid.NewGuid(), "x@y.z", [], "issuer", "audience", SigningKey, Lifetime, Now);

        Assert.Equal(3, token.Split('.').Length);
    }
}
