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
        [Required, StringLength(200, MinimumLength = 1)]
        public string Name { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal? TotaRevenue { get; set; }
        [Required]
        public int? DirectorId { get; set; }
        [Required, MinLength(1)] 
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

            var name = request.Name?.Trim();
            if (string.IsNullOrWhiteSpace(name))
                return Error("Name is required.");

            // unique name check 
            var nameExists = await Query()
                .AnyAsync(m => m.Id != request.Id && m.Name == name, cancellationToken);
            if (nameExists)
                return Error("Movie already exists.");

            // director must exist
            var directorExists = await Query<Director>()
                .AnyAsync(d => d.Id == request.DirectorId, cancellationToken);
            if (!directorExists)
                return Error("Director not found.");

            // genres must exist and be unique
            var distinctGenreIds = request.GenreIds
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            if (distinctGenreIds.Count == 0)
                return Error("At least one genre must be selected.");

            var existingGenreCount = await Query<Genre>()
                .CountAsync(g => distinctGenreIds.Contains(g.Id), cancellationToken);
            if (existingGenreCount != distinctGenreIds.Count)
                return Error("One or more genres were not found.");
            
            Delete(entity.MovieGenres);
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