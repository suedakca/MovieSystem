using CORE.APP.Models.Authentication;
using CORE.APP.Services;
using CORE.APP.Services.Authentication;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Users.APP.Domain;

namespace Users.APP.Features.Tokens
{
    public class RefreshTokenRequest : RefreshTokenRequestBase, IRequest<TokenResponse>
    {
    }

    public class RefreshTokenHandler : Service<User>, IRequestHandler<RefreshTokenRequest, TokenResponse>
    {
        private readonly ITokenAuthService _tokenAuthService;

        public RefreshTokenHandler(DbContext db, ITokenAuthService tokenAuthService) : base(db)
        {
            _tokenAuthService = tokenAuthService;
        }

        protected override IQueryable<User> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking)
                .Include(u => u.Group)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role);
        }

        public async Task<TokenResponse> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var claims = _tokenAuthService.GetClaims(request.Token, request.SecurityKey);
            
            var userIdClaim = claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
                return null;

            var userId = Convert.ToInt32(userIdClaim.Value);

            var user = await Query(false).SingleOrDefaultAsync(u =>
                    u.Id == userId &&
                    u.RefreshToken == request.RefreshToken &&
                    u.RefreshTokenExpiration >= DateTime.Now,
                cancellationToken);

            if (user is null)
                return null;

            user.RefreshToken = _tokenAuthService.GetRefreshToken();
            await Update(user, cancellationToken);

            var expiration = DateTime.Now.AddMinutes(5);

            return _tokenAuthService.GetTokenResponse(
                user.Id,
                user.UserName,
                user.UserRoles.Select(ur => ur.Role.Name).ToArray(),
                expiration,
                request.SecurityKey,
                request.Issuer,
                request.Audience,
                user.RefreshToken,
                user.Group?.Title ?? string.Empty
            );
        }
    }
}