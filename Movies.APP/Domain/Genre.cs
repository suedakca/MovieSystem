using System.ComponentModel.DataAnnotations;
using CORE.APP.Domain;

namespace Movies.APP.Domain
{

    public class Genre : Entity
    {
        [Required, StringLength(30)]
        public string Name { get; set; }
        public List<MovieGenre> MovieGenres { get; set; }
    }
}