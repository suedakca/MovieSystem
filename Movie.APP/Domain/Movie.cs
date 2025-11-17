using CORE.APP.Domain;

namespace Movie.APP.Domain;

public class Movie : Entity
{
    public int MovieId { get; set; }
    public string Name { get; set; }
    public DateTime ReleaseDate { get; set; }
    public decimal TotaRevenue { get; set; }
    public int DirectorId { get; set; }
    public List<MovieGenre> MovieGenres { get; set; }
    public Director Director { get; set; }
}