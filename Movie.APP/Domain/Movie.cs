namespace Movie.APP.Domain;

public class Movie : Entity
{
    public int MovieId { get; set; }
    public string Name { get; set; }
    public DateTime ReleaseDate { get; set; }
    public decimal TotalRevenue { get; set; }
    public int DirectorId { get; set; }
}