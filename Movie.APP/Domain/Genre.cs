using CORE.APP.Domain;

namespace Movie.APP.Domain;

public class Genre : Entity
{
    public int GenreId { get; set; }
    public string Name { get; set; }
    public List<MovieGenre> MovieGenres { get; set; }
}