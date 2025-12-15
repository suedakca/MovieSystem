using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Movies.APP.Domain;

namespace Movies.APP.Features.Movies
{
    public class MovieCreateRequest : Request, IRequest<CommandResponse>
    {
        [Required, StringLength(100)] 
        public string Name { get; set; }
        
        [Required, StringLength(100)]
        public Director Director { get; set; }
        
        public DateTime? ReleaseDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? TotaRevenue { get; set; }

        public int? DirectorId { get; set; } 
        
        [Required]
        public List<int> GenreIds { get; set; }
        
    }

    public class MovieCreateHandler : ServiceBase, IRequestHandler<MovieCreateRequest, CommandResponse>
    {
        private readonly MovieDb _db;
        public  MovieCreateHandler(MovieDb db)
        {
            _db = db;
        }
        public async Task<CommandResponse> Handle(MovieCreateRequest request, CancellationToken cancellationToken)
        {
            if (await _db.Movies.AnyAsync(movieEntity => movieEntity.Name == request.Name.Trim(), cancellationToken)) 
                return Error("Movie already exists");

            var entity = new Movie()
            {
                Name = request.Name.Trim(),
                Director = request.Director,
                ReleaseDate = request.ReleaseDate,
                TotaRevenue = request.TotaRevenue,
                DirectorId = request.DirectorId,
                GenreIds = request.GenreIds
            };
            
            _db.Movies.Add(entity);
            await _db.SaveChangesAsync(cancellationToken);
            return Success("Movie created successfully",  entity.Id);
        }
    }
}