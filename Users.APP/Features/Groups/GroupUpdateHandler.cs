using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Users.APP.Domain;

namespace Users.APP.Features.Groups
{
    public class GroupUpdateRequest : Request, IRequest<CommandResponse>
    {
        [Required, StringLength(100)]
        public string Title { get; set; }
    }
    
    public class GroupUpdateHandler : ServiceBase, IRequestHandler<GroupUpdateRequest, CommandResponse>
    {
        private readonly UsersDb _db;
        
        public GroupUpdateHandler(UsersDb db)
        {
            _db = db;
        }
        
        public async Task<CommandResponse> Handle(GroupUpdateRequest request, CancellationToken cancellationToken)
        {
            // If any other group (excluding the current one) already has the same title (case-sensitive, trimmed), don't update.
            // This prevents duplicate group names in the database.
            if (await _db.Groups.AnyAsync(groupEntity => groupEntity.Id != request.Id
                && groupEntity.Title == request.Title.Trim(), cancellationToken))
                return Error("Group with the same title exists!");

            // Attempt to find the group entity by its ID, if not found return error command response with message.
            var entity = await _db.Groups.FindAsync(request.Id, cancellationToken);
            if (entity is null)
                return Error("Group not found!");

            // Update the group's title with the new value (trimmed for consistency).
            entity.Title = request.Title.Trim(); // since request.Title has required data annotation and can't be null, assign request.Title's trimmed value

            // Mark the entity as modified in the context.
            _db.Groups.Update(entity); // _db.Update(entity); can also be written

            // Persist the changes to the database asynchronously by using Unit of Work (all changes made to the DbSets will be commited to the database once).
            await _db.SaveChangesAsync(cancellationToken);

            // Return a success response indicating the group was updated, including the entity's ID.
            return Success("Group updated successfully.", entity.Id);
        }
    }
}