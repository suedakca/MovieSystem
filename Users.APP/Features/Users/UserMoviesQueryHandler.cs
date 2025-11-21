using CORE.APP.Services;
using CORE.APP.Services.HTTP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Users.APP.Domain;

namespace Users.APP.Features.Users;

public class UserMoviesQueryHandler : Service<UserMovie>, IRequestHandler<UserMoviesQuery, List<UserMoviesResponse>>
{
    private readonly HttpServiceBase _http;

    public UserMoviesQueryHandler(DbContext db, HttpServiceBase http) : base(db)
    {
        _http = http;
    }

    public async Task<List<UserMoviesResponse>> Handle(UserMoviesQuery request, CancellationToken cancellationToken)
    {
        var userMovies = await Query()
            .Where(userMovie => userMovie.UserId == request.Id)
            .ToListAsync(cancellationToken);

        var result = new List<UserMoviesResponse>();

        foreach (var userMovie in userMovies)
        {
            // Movie API'den movie çek
            var movie = await _http.GetFromJson<UserMoviesResponse>(
                $"{request.MoviesApiUrl}/{userMovie.MovieId}", cancellationToken);

            result.Add(new UserMoviesResponse
            {
                Id = userMovie.UserId,
                MovieId = userMovie.MovieId,
                MovieName = userMovie.MovieName,
                Rating = userMovie.Rating,
                IsFavourite = userMovie.IsFavourite
            });
        }

        return result;
    }
}
