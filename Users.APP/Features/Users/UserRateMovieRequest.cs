using CORE.APP.Models;
using MediatR;

namespace Users.APP.Features.Users
{
    public class UserRateMovieRequest : Request, IRequest<CommandResponse>
    {
        public int UserId { get; set; }

        public int MovieId { get; set; }
        
        public int Rating { get; set; }
    }
}