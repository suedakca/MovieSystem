using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Users.APP.Domain;

namespace Users.APP.Features.Users
{
    public class UserRateMovieHandler 
        : Service<UserMovie>, IRequestHandler<UserRateMovieRequest, CommandResponse>
    {
        private readonly DbContext _db;

        public UserRateMovieHandler(DbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<CommandResponse> Handle(
            UserRateMovieRequest request,
            CancellationToken cancellationToken)
        {
            if (request.Rating < 1 || request.Rating > 10)
                return Error("Rating must be between 1 and 10.");

            var alreadyRated = await Query()
                .AnyAsync(um =>
                        um.UserId == request.UserId &&
                        um.MovieId == request.MovieId,
                    cancellationToken);

            if (alreadyRated)
                return Error("User already rated this movie.");

            await Create(new UserMovie
            {
                UserId = request.UserId,
                MovieId = request.MovieId,
                Rating = request.Rating,
                RatedDate = DateTime.Now
            }, cancellationToken);

            var user = await _db.Set<User>()
                .SingleAsync(u => u.Id == request.UserId, cancellationToken);

            user.Score += 10;
            await _db.SaveChangesAsync(cancellationToken);

            return Success("Movie rated successfully.", request.MovieId);
        }
    }
}