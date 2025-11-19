#nullable disable
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;

namespace Movies.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
    private readonly MovieDb _db; 
        private readonly IWebHostEnvironment _environment;
        public DatabaseController(MovieDb db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }
        [HttpGet, Route("~/api/SeedDb")]
        public IActionResult Seed()
        {
            if (_db.Movies.Any())
                _db.Movies.RemoveRange(_db.Movies.ToList());

            if (_db.Directors.Any())
                _db.Directors.RemoveRange(_db.Directors.ToList());

            if (_db.Genres.Any())
                _db.Genres.RemoveRange(_db.Genres.ToList());

            _db.Database.ExecuteSqlRaw("UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE NAME='Movies';");
            _db.Database.ExecuteSqlRaw("UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE NAME='Directors';");
            _db.Database.ExecuteSqlRaw("UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE NAME='Genres';");

            var genreAction     = new Genre { Guid = Guid.NewGuid().ToString(), Name = "Action" };
            var genreDrama      = new Genre { Guid = Guid.NewGuid().ToString(), Name = "Drama" };
            var genreSciFi      = new Genre { Guid = Guid.NewGuid().ToString(), Name = "Sci-Fi" };
            var genreComedy     = new Genre { Guid = Guid.NewGuid().ToString(), Name = "Comedy" };
            var genreAnimation  = new Genre { Guid = Guid.NewGuid().ToString(), Name = "Animation" };

            _db.Genres.AddRange(genreAction, genreDrama, genreSciFi, genreComedy, genreAnimation);

            var dirNolan = new Director
            {
                Guid = Guid.NewGuid().ToString(),
                FirstName = "Christopher",
                LastName = "Nolan"
            };

            var dirSpielberg = new Director
            {
                Guid = Guid.NewGuid().ToString(),
                FirstName = "Steven",
                LastName = "Spielberg"
            };

            var dirVilleneuve = new Director
            {
                Guid = Guid.NewGuid().ToString(),
                FirstName = "Denis",
                LastName = "Villeneuve"
            };

            var dirMiyazaki = new Director
            {
                Guid = Guid.NewGuid().ToString(),
                FirstName = "Hayao",
                LastName = "Miyazaki"
            };

            _db.Directors.AddRange(dirNolan, dirSpielberg, dirVilleneuve, dirMiyazaki);
            _db.Directors.AddRange(dirNolan, dirSpielberg, dirVilleneuve, dirMiyazaki);
            _db.SaveChanges(); 
            
            var inception = new APP.Domain.Movie()
            {
                Guid = Guid.NewGuid().ToString(),
                Name = "Inception",
                ReleaseDate = new DateTime(2010, 7, 16),
                TotaRevenue = 829895144m, 
                DirectorId = dirNolan.Id,
                GenreIds = new List<int> { genreSciFi.Id }
            };

            var interstellar = new Movie
            {
                Guid = Guid.NewGuid().ToString(),
                Name = "Interstellar",
                ReleaseDate = new DateTime(2014, 11, 7),
                TotaRevenue = 677471339m,
                DirectorId = dirNolan.Id,
                GenreIds = new List<int> { genreSciFi.Id }
            };

            var jurassicPark = new Movie
            {
                Guid = Guid.NewGuid().ToString(),
                Name = "Jurassic Park",
                ReleaseDate = new DateTime(1993, 6, 11),
                TotaRevenue = 1040000000m, // yaklaşık, istersen değiştir/sil
                DirectorId = dirSpielberg.Id,
                GenreIds = new List<int> { genreAction.Id }
            };

            var dune = new Movie
            {
                Guid = Guid.NewGuid().ToString(),
                Name = "Dune",
                ReleaseDate = new DateTime(2021, 10, 22),
                TotaRevenue = 406000000m,
                DirectorId = dirVilleneuve.Id,
                GenreIds = new List<int> { genreSciFi.Id }
            };

            var spiritedAway = new Movie
            {
                Guid = Guid.NewGuid().ToString(),
                Name = "Spirited Away",
                ReleaseDate = new DateTime(2001, 7, 20),
                TotaRevenue = 395800000m,
                DirectorId = dirMiyazaki.Id,
                GenreIds = new List<int> { genreAnimation.Id }
            };

            _db.Movies.AddRange(inception, interstellar, jurassicPark, dune, spiritedAway);

            _db.Movies.AddRange(inception, interstellar, jurassicPark, dune, spiritedAway);

            // Değişiklikleri kaydet
            _db.SaveChanges();

            return Ok("Movie database seed successful.");
        }
    }
}