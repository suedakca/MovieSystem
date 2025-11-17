using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CORE.APP.Models.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CORE.APP.Services.Authentication;

public class TokenAuthService: AuthServiceBase, ITokenAuthService
{
    public TokenResponse GetTokenResponse(int userId, string userName, string[] userRoleNames, DateTime expiration, 
            string securityKey, string issuer, string audience, string refreshToken)
        {
            // Generate claims.
            var claims = GetClaims(userId, userName, userRoleNames);

            // Create signing credentials using the provided security key.
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // Build the JWT with claims, issuer, audience, and expiration.
            var jwtSecurityToken = new JwtSecurityToken(issuer, audience, claims, DateTime.Now, expiration, signingCredentials);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            // Serialize the JWT to a string.
            var jwt = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);

            // Return the token response with the serialized JWT value and the refresh token parameter value.
            return new TokenResponse()
            {
                Token = $"{JwtBearerDefaults.AuthenticationScheme} {jwt}", // JwtBearerDefaults.AuthenticationScheme: "Bearer"
                RefreshToken = refreshToken
            };
        }

        public string GetRefreshToken()
        {
            // Generate a cryptographically secure random 32-byte refresh token.
            var bytes = new byte[32];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }
        
        public IEnumerable<Claim> GetClaims(string token, string securityKey)
        {
           
            token = token.StartsWith(JwtBearerDefaults.AuthenticationScheme) ? 
                token.Remove(0, JwtBearerDefaults.AuthenticationScheme.Length + 1) : token;

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey
            };

            // Validate the JWT and extract claims.
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            return securityToken is null ? null : principal.Claims;
        }
}