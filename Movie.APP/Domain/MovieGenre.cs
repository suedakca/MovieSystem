namespace Movie.APP.Domain;

public class MovieGenre : Entity
{
    public int MovieGenreId { get; set; }
    public int GenreId { get; set; }
    public int MovieId { get; set; }
}