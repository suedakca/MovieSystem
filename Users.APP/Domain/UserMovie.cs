using CORE.APP.Domain;

namespace Users.APP.Domain;

public class UserMovie : Entity
{
    public int UserId { get; set; }
    public int MovieId { get; set; }
    public String MovieName { get; set; }
    public bool IsFavourite { get; set; }
    public DateTime? RatedDate { get; set; }
    public int? Rating { get; set; }
    public User User { get; set; }
}