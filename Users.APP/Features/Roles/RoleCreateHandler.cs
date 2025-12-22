using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Users.APP.Domain;

namespace Users.APP.Features.Roles
{
    public class RoleCreateRequest : Request, IRequest<CommandResponse>
    {
        [Required, StringLength(25)]
        public string Name { get; set; }
    }

    public class RoleCreateHandler : Service<Role>, IRequestHandler<RoleCreateRequest, CommandResponse>
    {
        public RoleCreateHandler(DbContext db) : base(db)
        {
        }

        public async Task<CommandResponse> Handle(RoleCreateRequest request, CancellationToken cancellationToken)
        {
            // check if a role with the same name exists.
            if (await Query().AnyAsync(r => r.Name == request.Name.Trim(), cancellationToken))
                return Error("Role with the same name exists!");

            var entity = new Role()
            {
                Name = request.Name.Trim() 
            };

            Create(entity); 

            return Success("Role created successfully.", entity.Id);
        }
    }
}