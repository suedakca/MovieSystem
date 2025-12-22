using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Users.APP.Domain;

namespace Users.APP.Features.Roles
{
    public class RoleDeleteRequest : Request, IRequest<CommandResponse>
    {
    }

    public class RoleDeleteHandler : Service<Role>, IRequestHandler<RoleDeleteRequest, CommandResponse>
    {
        public RoleDeleteHandler(DbContext db) : base(db)
        {
        }
        
        protected override IQueryable<Role> Query(bool isNoTracking = true)
        {
            return base.Query().Include(r => r.UserRoles);
        }

        public async Task<CommandResponse> Handle(RoleDeleteRequest request, CancellationToken cancellationToken)
        {
            var entity = await Query(false).SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
            if (entity is null)
                return Error("Role not found!");

            // delete the relational UserRole entities data
            Delete(entity.UserRoles);

            // delete the Role entity data
            Delete(entity); 

            return Success("Role deleted successfully.", entity.Id);
        }
    }
}