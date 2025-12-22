using CORE.APP.Services;
using CORE.APP.Services.Authentication;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Users.APP.Domain;

namespace Users.APP.Features.Auth
{
    public class RegisterHandler : Service<User>, IRequestHandler<RegisterRequest, RegisterResponse>
    {
        private readonly DbContext _db;
        private readonly ITokenAuthService _tokenAuthService;
        private readonly IConfiguration _configuration;

        public RegisterHandler(
            DbContext db,
            ITokenAuthService tokenAuthService,
            IConfiguration configuration
        ) : base(db)
        {
            _db = db;
            _tokenAuthService = tokenAuthService;
            _configuration = configuration;
        }

        public async Task<RegisterResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
        {
            if (await _db.Set<User>().AnyAsync(u => u.UserName == request.UserName, cancellationToken))
                return new RegisterResponse(false, "UserName already exists.");

            // YAŞ HESABI
            var age = CalculateAge(request.BirthDate);
            var groupTitle = age < 18 ? "Child" : "Adult";

            var group = await _db.Set<Group>()
                .SingleOrDefaultAsync(g => g.Title == groupTitle, cancellationToken);

            if (group == null)
                return new RegisterResponse(false, "Groups not seeded.");

            var role = await _db.Set<Role>()
                .SingleOrDefaultAsync(r => r.Name == "User", cancellationToken);

            if (role == null)
                return new RegisterResponse(false, "Roles not seeded.");

            var refreshToken = _tokenAuthService.GetRefreshToken();

            var user = new User
            {
                UserName = request.UserName,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Address = request.Address,
                Gender = request.Gender,
                BirthDate = request.BirthDate,
                RegistrationDate = DateTime.Now,
                Score = 0,
                IsActive = true,
                GroupId = group.Id,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = DateTime.Now.AddDays(7)
            };

            user.UserRoles.Add(new UserRole { RoleId = role.Id });

            await Create(user, cancellationToken);

            var token = _tokenAuthService.GetTokenResponse(
                user.Id,
                user.UserName,
                user.UserRoles.Select(r => r.Role.Name).ToArray(),
                DateTime.Now.AddMinutes(5),
                _configuration["SecurityKey"],
                _configuration["Issuer"],
                _configuration["Audience"],
                refreshToken,
                group.Title
            );

            return new RegisterResponse(true, "Registered")
            {
                Token = token.Token,
                RefreshToken = token.RefreshToken
            };
        }

        private static int CalculateAge(DateOnly birthDate)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age)) age--;
            return age;
        }
    }
}