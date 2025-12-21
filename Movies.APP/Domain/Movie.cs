using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CORE.APP.Domain;

namespace Movies.APP.Domain
{
    public class Movie : Entity
    {
        [Required, StringLength(30)]
        public string Name { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public decimal? TotaRevenue { get; set; }
        public int? DirectorId { get; set; }
        public List<MovieGenre> MovieGenres { get; set; }
        public Director Director { get; set; }
        
        [NotMapped]
        public List<int> GenreIds 
        {
            get => MovieGenres.Select(mgEntity => mgEntity.GenreId).ToList();
            set => MovieGenres = value.Select(genreId => new MovieGenre() { GenreId = genreId }).ToList();
        }
    }
}