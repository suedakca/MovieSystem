using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CORE.APP.Models.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CORE.APP.Services.Authentication;

public class TokenAuthService : AuthServiceBase, ITokenAuthService
{
    public TokenResponse GetTokenResponse(
        int userId,
        string userName,
        string[] userRoleNames,
        DateTime expiration,
        string securityKey,
        string issuer,
        string audience,
        string refreshToken,
        string groupTitle //(Child / Adult)
    )
    {
        var claims = GetClaims(userId, userName, userRoleNames, groupTitle);

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            DateTime.Now,
            expiration,
            signingCredentials
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        return new TokenResponse
        {
            Token = $"{JwtBearerDefaults.AuthenticationScheme} {jwt}",
            RefreshToken = refreshToken
        };
    }
    
    protected IEnumerable<Claim> GetClaims(
        int userId,
        string userName,
        string[] roleNames,
        string groupTitle
    )
    {
        Console.WriteLine("GROUP TITLE: " + groupTitle);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, userName),
            new Claim("groupTitle", groupTitle)
        };

        // ROLE CLAIMS
        foreach (var role in roleNames)
            claims.Add(new Claim(ClaimTypes.Role, role));

        return claims;
    }
    
    public string GetRefreshToken()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
    
    public IEnumerable<Claim> GetClaims(string token, string securityKey)
    {
        token = token.StartsWith(JwtBearerDefaults.AuthenticationScheme)
            ? token[(JwtBearerDefaults.AuthenticationScheme.Length + 1)..]
            : token;

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey
        };

        var handler = new JwtSecurityTokenHandler();
        handler.ValidateToken(token, parameters, out var securityToken);

        return securityToken is null
            ? Enumerable.Empty<Claim>()
            : ((JwtSecurityToken)securityToken).Claims;
    }
}