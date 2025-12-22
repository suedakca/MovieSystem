using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Users.APP.Domain;

namespace Users.APP.Features.Groups
{
    public class GroupCreateRequest : Request, IRequest<CommandResponse>
    {
        [Required, StringLength(100)]
        public string Title { get; set; }
    }
    
    public class GroupCreateHandler : ServiceBase, IRequestHandler<GroupCreateRequest, CommandResponse>
    {
        private readonly UsersDb _db;
        public GroupCreateHandler(UsersDb db)
        {
            _db = db;
        }
        public async Task<CommandResponse> Handle(GroupCreateRequest request, CancellationToken cancellationToken)
        {
            if (await _db.Groups.AnyAsync(groupEntity => groupEntity.Title == request.Title.Trim(), cancellationToken))
                return Error("Group with the same title exists!");

            // Creates a new Group entity with the provided title
            var entity = new Group()
            {
                Title = request.Title.Trim() 
            };

            // Adds the new group entity to the database context.
            _db.Groups.Add(entity);

            // Saves changes to the database asynchronously
            await _db.SaveChangesAsync(cancellationToken);

            return Success("Group created successfully.", entity.Id);
        }
    }
}