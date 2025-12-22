using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Users.APP.Domain;

namespace Users.APP.Features.Roles
{
    public class RoleQueryRequest : Request, IRequest<IQueryable<RoleQueryResponse>>
    {
    }

    public class RoleQueryResponse : Response
    {
        public string Name { get; set; }
        public int UserCount { get; set; }
        public string Users { get; set; }
    }
    
    public class RoleQueryHandler : Service<Role>, IRequestHandler<RoleQueryRequest, IQueryable<RoleQueryResponse>>
    {
        public RoleQueryHandler(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Role> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking) 
                .Include(r => r.UserRoles).ThenInclude(ur => ur.User) 
                .OrderBy(r => r.Name); 
        }

        public Task<IQueryable<RoleQueryResponse>> Handle(RoleQueryRequest request, CancellationToken cancellationToken)
        {
            var query = Query().Select(r => new RoleQueryResponse()
            {
                Id = r.Id,
                Guid = r.Guid,
                Name = r.Name,

                UserCount = r.UserRoles.Count, 
                Users = string.Join(", ", r.UserRoles.Select(ur => ur.User.UserName))
            });

            return Task.FromResult(query);
        }
    }
}