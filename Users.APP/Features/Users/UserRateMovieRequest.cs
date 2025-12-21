using System.Text.Json.Serialization;
using CORE.APP.Models;
using MediatR;

namespace Users.APP.Features.Users
{
    public class UserRateMovieRequest :  IRequest<CommandResponse>
    {
        [JsonIgnore]
        public int UserId { get; set; }

        public int MovieId { get; set; }
        
        public int Rating { get; set; }
    }
}