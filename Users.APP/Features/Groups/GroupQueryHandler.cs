using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Users.APP.Domain;

namespace Users.APP.Features.Groups
{
    public class GroupQueryRequest : Request, IRequest<IQueryable<GroupQueryResponse>>
    {
    }
    public class GroupQueryResponse : Response
    {
        public string Title { get; set; }
    }
    
    public class GroupQueryHandler : ServiceBase, IRequestHandler<GroupQueryRequest, IQueryable<GroupQueryResponse>>
    {
        
        private readonly UsersDb _db; 
        
        public GroupQueryHandler(UsersDb db)
        {
            _db = db;
        }

       
        public Task<IQueryable<GroupQueryResponse>> Handle(GroupQueryRequest request, CancellationToken cancellationToken)
        {
            var query = _db.Groups.Select(groupEntity => new GroupQueryResponse()
            {
                Id = groupEntity.Id,         // Maps the entity's integer ID.
                Guid = groupEntity.Guid,     // Maps the entity's GUID.
                Title = groupEntity.Title    // Maps the entity's title to the response.
            });
            
            return Task.FromResult(query);
        }
    }
}