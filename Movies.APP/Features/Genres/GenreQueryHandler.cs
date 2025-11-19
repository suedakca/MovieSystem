using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Movies.APP.Domain;

namespace Movies.APP.Features.Genres
{
    public class GenreQueryRequest : Request, IRequest<IQueryable<GenreQueryResponse>>
    {
    }

    public class GenreQueryResponse : Response
    {
        public string Name { get; set; }
    }

    public class GenreQueryHandler : ServiceBase, IRequestHandler<GenreQueryRequest, IQueryable<GenreQueryResponse>>
    {
        private readonly MovieDb _db;

        public GenreQueryHandler(MovieDb db)
        {
            _db = db;
        }

        public Task<IQueryable<GenreQueryResponse>> Handle(GenreQueryRequest request, CancellationToken cancellationToken)
        {
            var query = _db.Genres.Select(g => new GenreQueryResponse
            {
                Id = g.Id,
                Guid = g.Guid,
                Name = g.Name
            });

            return Task.FromResult(query);
        }
    }
}