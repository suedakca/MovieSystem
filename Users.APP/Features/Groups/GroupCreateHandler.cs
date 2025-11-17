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
            
            var entity = new Group()
            {
                Title = request.Title.Trim() // since request.Title has required data annotation and can't be null, assign request.Title's trimmed value
            };
            
            _db.Groups.Add(entity); 
            
            await _db.SaveChangesAsync(cancellationToken);

            // Returns a success response indicating the group was created with the created group entity's Id value.
            return Success("Group created successfully.", entity.Id);
        }
    }
}