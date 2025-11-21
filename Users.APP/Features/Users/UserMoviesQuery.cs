using System.Text.Json.Serialization;
using CORE.APP.Models;
using MediatR;

namespace Users.APP.Features.Users;

public class UserMoviesQuery : Request, IRequest<List<UserMoviesResponse>>
{
    [JsonIgnore]
    public string MoviesApiUrl { get; set; }
}