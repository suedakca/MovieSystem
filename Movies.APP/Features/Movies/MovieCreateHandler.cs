using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Movies.APP.Domain;

namespace Movies.APP.Features.Movies
{
    public class MovieCreateRequest : IRequest<CommandResponse>
    {
        [Required, StringLength(100, MinimumLength = 1)]
        public string Name { get; set; }

        public DateOnly? ReleaseDate { get; set; }

        public decimal? TotaRevenue { get; set; }

        [Required]
        public int DirectorId { get; set; }

        [Required]
        public List<int> GenreIds { get; set; }
    }

    public class MovieCreateHandler : ServiceBase, IRequestHandler<MovieCreateRequest, CommandResponse>
    {
        private readonly MovieDB _db;
        public  MovieCreateHandler(MovieDB db)
        {
            _db = db;
        }
        public async Task<CommandResponse> Handle(MovieCreateRequest request, CancellationToken cancellationToken)
        {
            var name = request.Name?.Trim();
            if (string.IsNullOrWhiteSpace(name))
                return Error("Name is required.");

            if (await _db.Movies.AnyAsync(movieEntity => movieEntity.Name == name, cancellationToken)) 
                return Error("Movie already exists");

            // director must exist
            var directorExists = await _db.Directors.AnyAsync(d => d.Id == request.DirectorId, cancellationToken);
            if (!directorExists)
                return Error("Director not found.");

            // genres must exist and be unique
            var distinctGenreIds = request.GenreIds
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            if (distinctGenreIds.Count == 0)
                return Error("At least one genre must be selected.");

            var existingGenreCount = await _db.Genres
                .CountAsync(g => distinctGenreIds.Contains(g.Id), cancellationToken);

            if (existingGenreCount != distinctGenreIds.Count)
                return Error("One or more genres were not found.");

            var entity = new Movie()
            {
                Name = name,
                ReleaseDate = request.ReleaseDate,
                TotaRevenue = request.TotaRevenue,
                DirectorId = request.DirectorId,
                GenreIds = distinctGenreIds
            };
            
            _db.Movies.Add(entity);
            await _db.SaveChangesAsync(cancellationToken);
            return Success("Movie created successfully", entity.Id);
        }
    }
}