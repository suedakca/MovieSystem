using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;

namespace Movies.APP.Features.Movies;

public class CustomerMoviesQuery : Request, IRequest<List<CustomerMovieResponse>>
{
    public bool IsChildGroup { get; set; }
    public int? MovieId { get; set; }
}

public class CustomerMovieResponse : Response
{
    public string Name { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public int? DirectorId { get; set; }
    public string DirectorFirstName { get; set; }
    public string DirectorLastName { get; set; }
    public List<string> Genres { get; set; } = new();
}

public class CustomerMovieQueryHandler
    : Service<Movie>, IRequestHandler<CustomerMoviesQuery, List<CustomerMovieResponse>>
{
    private const string ChildGenreName = "Child";

    public CustomerMovieQueryHandler(DbContext db) : base(db)
    {
    }

    protected override IQueryable<Movie> Query(bool isNoTracking = true)
    {
        return base.Query(isNoTracking)
            .Include(m => m.Director)
            .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
            .OrderBy(m => m.Name)
            .ThenBy(m => m.ReleaseDate);
    }

    public async Task<List<CustomerMovieResponse>> Handle(
        CustomerMoviesQuery request,
        CancellationToken cancellationToken)
    {
        var entityQuery = Query();

        if (request.MovieId.HasValue)
        {
            entityQuery = entityQuery.Where(m => m.Id == request.MovieId.Value);
        }

        entityQuery = request.IsChildGroup
            ? entityQuery.Where(m =>
                m.MovieGenres.Any(mg => mg.Genre != null && mg.Genre.Name == ChildGenreName))
            : entityQuery.Where(m =>
                m.MovieGenres.All(mg => mg.Genre != null && mg.Genre.Name != ChildGenreName));

        var movies = await entityQuery.ToListAsync(cancellationToken);

        return movies.Select(m => new CustomerMovieResponse
        {
            Id = m.Id,
            Guid = m.Guid,
            Name = m.Name,
            ReleaseDate = m.ReleaseDate,
            DirectorId = m.DirectorId,
            DirectorFirstName = m.Director?.FirstName ?? string.Empty,
            DirectorLastName = m.Director?.LastName ?? string.Empty,
            Genres = m.MovieGenres?
                .Where(mg => mg.Genre != null)
                .Select(mg => mg.Genre!.Name)
                .ToList() ?? new List<string>()
        }).ToList();
    }
}
