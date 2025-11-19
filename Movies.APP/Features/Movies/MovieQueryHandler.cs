using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;

namespace Movies.APP.Features.Movies
{
    public class MovieQueryRequest : Request, IRequest<IQueryable<MovieQueryResponse>>
    {
        public string Name { get; set; }
        public DateTime? ReleaseDateStart { get; set; }
        public DateTime? ReleaseDateEnd { get; set; }
        public decimal? TotalRevenueStart { get; set; }
        public decimal? TotalRevenueEnd { get; set; }
        public int? DirectorId { get; set; }
        public List<int> GenreIds { get; set; } = new List<int>();
    }
    
    public class MovieQueryResponse : Response
    {
        public string Name { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public decimal? TotaRevenue { get; set; }
        public int? DirectorId { get; set; }
        public List<int> GenreIds { get; set; }

        // add the new properties, some ending with F for the properties with the same name, for custom or formatted string values
        public string ReleaseDateF { get; set; }
        public string TotaRevenueF { get; set; }
        public string Director { get; set; }
        public List<string> Genres { get; set; }
    }

    public class MovieQueryHandler : Service<Movie>, IRequestHandler<MovieQueryRequest, IQueryable<MovieQueryResponse>>
    {
        public MovieQueryHandler(DbContext db) : base(db)
        {
        }
        
        protected override IQueryable<Movie> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking)
                .Include(m => m.Director)
                .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre) 
                .OrderBy(m => m.Name) 
                .ThenBy(m => m.ReleaseDate);
        }

        public Task<IQueryable<MovieQueryResponse>> Handle(MovieQueryRequest request, CancellationToken cancellationToken)
        {
            var entityQuery = Query();
            if (!string.IsNullOrWhiteSpace(request.Name))
                entityQuery = entityQuery.Where(m => m.Name.Contains(request.Name.Trim()));
            
            if (request.ReleaseDateStart.HasValue)
                entityQuery = entityQuery.Where(m => m.ReleaseDate.HasValue &&
                                                     m.ReleaseDate.Value.Date >= request.ReleaseDateStart.Value.Date);
            
            if (request.ReleaseDateEnd.HasValue)
                entityQuery = entityQuery.Where(m => m.ReleaseDate.HasValue &&
                                                     m.ReleaseDate.Value.Date <= request.ReleaseDateEnd.Value.Date);
            
            if (request.TotalRevenueStart.HasValue)
                entityQuery = entityQuery.Where(m => m.TotaRevenue >= request.TotalRevenueStart.Value);
            
            if (request.TotalRevenueEnd.HasValue)
                entityQuery = entityQuery.Where(m => m.TotaRevenue <= request.TotalRevenueEnd.Value);
            
            if (request.DirectorId.HasValue)
                entityQuery = entityQuery.Where(m => m.DirectorId == request.DirectorId.Value);

           
            if (request.GenreIds.Count > 0)
                entityQuery = entityQuery.Where(m => m.MovieGenres.Any(mg => request.GenreIds.Contains(mg.GenreId)));
            
            var query = entityQuery.Select(m => new MovieQueryResponse
            {
                Id = m.Id,
                Guid = m.Guid,
                Name = m.Name,
                ReleaseDate = m.ReleaseDate,
                TotaRevenue = m.TotaRevenue,
                DirectorId = m.DirectorId,
                GenreIds = m.MovieGenres.Select(mg => mg.GenreId).ToList(),
                
                ReleaseDateF = m.ReleaseDate.HasValue
                    ? m.ReleaseDate.Value.ToString("MM/dd/yyyy")
                    : string.Empty,

                TotaRevenueF = m.TotaRevenue.ToString(), 

                Director = m.Director != null ? m.Director.FirstName : null,

                Genres = m.MovieGenres
                    .Select(mg => mg.Genre.Name)
                    .ToList() 
            });
            
            return Task.FromResult(query);
        }
    }
}