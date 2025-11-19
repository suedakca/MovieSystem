using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Movies.APP.Domain;

namespace Movies.APP.Features.Genres
{
    public class GenreUpdateRequest : Request, IRequest<CommandResponse>
    {
        [Required, StringLength(100)]
        public string Name { get; set; }
    }

    public class GenreUpdateHandler : ServiceBase, IRequestHandler<GenreUpdateRequest, CommandResponse>
    {
        private readonly MovieDb _db;

        public GenreUpdateHandler(MovieDb db)
        {
            _db = db;
        }

        public async Task<CommandResponse> Handle(GenreUpdateRequest request, CancellationToken cancellationToken)
        {
            if (await _db.Genres.AnyAsync(
                    g => g.Id != request.Id && g.Name == request.Name.Trim(),
                    cancellationToken))
                return Error("Genre with the same name exists!");

            var entity = await _db.Genres.FindAsync(request.Id, cancellationToken);
            if (entity is null)
                return Error("Genre not found!");

            entity.Name = request.Name.Trim();

            _db.Genres.Update(entity);
            await _db.SaveChangesAsync(cancellationToken);

            return Success("Genre updated successfully.", entity.Id);
        }
    }
}