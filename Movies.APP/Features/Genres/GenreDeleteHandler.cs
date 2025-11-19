using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;

namespace Movies.APP.Features.Genres
{
    public class GenreDeleteRequest : Request, IRequest<CommandResponse>
    {
    }

    public class GenreDeleteHandler : ServiceBase, IRequestHandler<GenreDeleteRequest, CommandResponse>
    {
        private readonly MovieDb _db;

        public GenreDeleteHandler(MovieDb db)
        {
            _db = db;
        }

        public async Task<CommandResponse> Handle(GenreDeleteRequest request, CancellationToken cancellationToken)
        {
            var entity = await _db.Genres
                .Include(g => g.MovieGenres)
                .SingleOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

            if (entity is null)
                return Error("Genre not found!");

            if (entity.MovieGenres.Count > 0)
                return Error("Genre can't be deleted because it has relational movies!");

            _db.Genres.Remove(entity);
            await _db.SaveChangesAsync(cancellationToken);

            return Success("Genre deleted successfully.", entity.Id);
        }
    }
}