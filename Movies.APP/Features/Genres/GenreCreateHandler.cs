using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Movies.APP.Domain;

namespace Movies.APP.Features.Genres
{
    public class GenreCreateRequest : Request, IRequest<CommandResponse>
    {
        [Required, StringLength(100)]
        public string Name { get; set; }
    }
    public class GenreCreateHandler : ServiceBase, IRequestHandler<GenreCreateRequest, CommandResponse>
    {
        private readonly MovieDb _db;
        public GenreCreateHandler(MovieDb db)
        {
            _db = db;
        }
        
        public async Task<CommandResponse> Handle(GenreCreateRequest request, CancellationToken cancellationToken)
        {
            if (await _db.Genres.AnyAsync(
                    genreEntity => genreEntity.Name == request.Name.Trim(),
                    cancellationToken))
            {
                return Error("Genre with the same name exists!");
            }

            var entity = new Genre()
            {
                Name = request.Name.Trim()
            };

            _db.Genres.Add(entity);

            await _db.SaveChangesAsync(cancellationToken);
            return Success("Genre created successfully.", entity.Id);
        }
    }
}