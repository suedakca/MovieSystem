using CORE.APP.Domain;

namespace Movies.APP.Domain
{

    public class Genre : Entity
    {
        public int GenreId { get; set; }
        public string? Name { get; set; }
        public List<MovieGenre>? MovieGenres { get; set; }
    }
}