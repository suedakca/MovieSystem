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
        [Required, StringLength(100)] public string Name { get; set; }
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

            var entity = new Domain.Movie()
            {
                Name = request.Name.Trim()
            };
            
            _db.Movies.Add(entity);
            await _db.SaveChangesAsync(cancellationToken);
            return Success("Movie created successfully",  entity.Id);
        }
    }
}