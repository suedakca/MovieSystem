using CORE.APP.Models;

namespace Users.APP.Features.Users;

public class UserMoviesResponse : Response
{
    public int MovieId { get; set; }
    public string MovieName { get; set; }
    public bool IsFavourite { get; set; }
    public int? Rating { get; set; }
}