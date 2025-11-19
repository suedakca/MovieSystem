using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;

namespace Movies.APP.Features.Directors
{
    public class DirectorQueryRequest : Request, IRequest<IQueryable<DirectorQueryResponse>>
    {
    }

    public class DirectorQueryResponse : Response
    {
        public string FirstName { get; set; }
        public string LastName  { get; set; }
        public bool IsRetired   { get; set; }
        public string FullName  { get; set; }
    }

    public class DirectorQueryHandler : ServiceBase, IRequestHandler<DirectorQueryRequest, IQueryable<DirectorQueryResponse>>
    {
        private readonly MovieDb _db;

        public DirectorQueryHandler(MovieDb db)
        {
            _db = db;
        }

        public Task<IQueryable<DirectorQueryResponse>> Handle(DirectorQueryRequest request, CancellationToken cancellationToken)
        {
            var query = _db.Directors.Select(d => new DirectorQueryResponse
            {
                Id        = d.Id,
                Guid      = d.Guid,
                FirstName = d.FirstName,
                LastName  = d.LastName,
                IsRetired = d.IsRetired,
                FullName  = d.FirstName + " " + d.LastName
            });

            return Task.FromResult(query);
        }
    }
}