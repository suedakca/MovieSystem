using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Movies.APP.Domain;

namespace Movies.APP.Features.Movies
{
    public class MovieUpdateRequest : Request, IRequest<CommandResponse>
    {
        public int MovieId { get; set; }
        [Required, StringLength(200, MinimumLength = 1)]
        public string Name { get; set; }
        public DateTime? ReleaseDate { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal? TotaRevenue { get; set; }
        public int? DirectorId { get; set; }
        public List<int> GenreIds { get; set; } = new List<int>();
    }

    public class MovieUpdateHandler : Service<Movie>, IRequestHandler<MovieUpdateRequest, CommandResponse>
    {
        public MovieUpdateHandler(DbContext db) : base(db)
        {
        }
        protected override IQueryable<Movie> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking)
                .Include(m => m.MovieGenres);
        }

        public async Task<CommandResponse> Handle(MovieUpdateRequest request, CancellationToken cancellationToken)
        {
            var entity = await Query(false)
                .SingleOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (entity is null)
                return Error("Movie not found!");

            Delete(entity.MovieGenres);
            
            entity.MovieId = request.MovieId;
            entity.Name = request.Name;
            entity.ReleaseDate = request.ReleaseDate;
            entity.TotaRevenue = request.TotaRevenue;
            entity.DirectorId = request.DirectorId;
            entity.GenreIds = request.GenreIds; 
            Update(entity);

            return Success("Movie updated successfully.", entity.Id);
        }
    }
}