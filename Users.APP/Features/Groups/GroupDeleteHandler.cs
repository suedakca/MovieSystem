using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Users.APP.Domain;

namespace Users.APP.Features.Groups
{
    public class GroupDeleteRequest : Request, IRequest<CommandResponse>
    {
    }
    
    public class GroupDeleteHandler : ServiceBase, IRequestHandler<GroupDeleteRequest, CommandResponse>
    {
        private readonly UsersDb _db;
        public GroupDeleteHandler(UsersDb db)
        {
            _db = db;
        }
        public async Task<CommandResponse> Handle(GroupDeleteRequest request, CancellationToken cancellationToken)
        {
            
            var entity = await _db.Groups.Include(groupEntity => groupEntity.Users)
                .SingleOrDefaultAsync(groupEntity => groupEntity.Id == request.Id, cancellationToken);

            // If the group does not exist, return an error command response.
            if (entity is null)
                return Error("Group not found!");

            // After the implementation of User entity
            if (entity.Users.Count > 0) 
                return Error("Group can't be deleted because it has relational users!"); 

            // Remove the group entity from the database context
            _db.Groups.Remove(entity); 

            await _db.SaveChangesAsync(cancellationToken);

            return Success("Group deleted successfully.", entity.Id);
        }
    }
}