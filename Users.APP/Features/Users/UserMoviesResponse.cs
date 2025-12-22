using System.Text.Json.Serialization;
using CORE.APP.Models;

namespace Users.APP.Features.Users;

public class UserMoviesResponse : Response
{
    [JsonIgnore]
    public int MovieId { get; set; }
    public string MovieName { get; set; }
    public int? Rating { get; set; }
}