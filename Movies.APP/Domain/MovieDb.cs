using Microsoft.EntityFrameworkCore;

namespace Movies.APP.Domain
{

    public class MovieDB : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Director> Directors { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }

        public MovieDB(DbContextOptions<MovieDB> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>().HasIndex(movieEntity => movieEntity.Name).IsUnique();
            modelBuilder.Entity<Genre>().HasIndex(genreEntity => genreEntity.Name).IsUnique();
            modelBuilder.Entity<Director>()
                .HasIndex(directorEntity => new { directorEntity.FirstName, directorEntity.LastName });

            modelBuilder.Entity<MovieGenre>()
                .HasOne(movieGenreEntity => movieGenreEntity.Movie)
                .WithMany(movieEntity => movieEntity.MovieGenres)
                .HasForeignKey(movieGenreEntity => movieGenreEntity.MovieId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MovieGenre>()
                .HasOne(movieGenreEntity => movieGenreEntity.Genre)
                .WithMany(genreEntity => genreEntity.MovieGenres)
                .HasForeignKey(movieGenreEntity => movieGenreEntity.GenreId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Movie>()
                .HasOne(movieEntity => movieEntity.Director)
                .WithMany(directorEntity => directorEntity.Movies)
                .HasForeignKey(movieEntity => movieEntity.DirectorId)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<MovieGenre>()
                .HasKey(x => new { x.MovieId, x.GenreId });

            modelBuilder.Entity<Director>().HasData(
                new Director { Id = 1, FirstName = "Christopher", LastName = "Nolan" },
                new Director { Id = 2, FirstName = "Quentin", LastName = "Tarantino" },
                new Director { Id = 3, FirstName = "Greta", LastName = "Gerwig" },
                new Director { Id = 4, FirstName = "John", LastName = "Lasseter", IsRetired = true }
            );

            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Action" },
                new Genre { Id = 2, Name = "Drama" },
                new Genre { Id = 3, Name = "Comedy" },
                new Genre { Id = 4, Name = "Sci-Fi" },
                new Genre { Id = 5, Name = "Child" }
            );

            modelBuilder.Entity<Movie>().HasData(
                new Movie
                {
                    Id = 1,
                    Name = "Inception",
                    DirectorId = 1,
                    ReleaseDate = new DateTime(2010, 7, 16),
                    TotaRevenue = 829895144
                },
                new Movie
                {
                    Id = 2,
                    Name = "Pulp Fiction",
                    DirectorId = 2,
                    ReleaseDate = new DateTime(1994, 10, 14),
                    TotaRevenue = 213928762
                },
                new Movie
                {
                    Id = 3,
                    Name = "Barbie",
                    DirectorId = 3,
                    ReleaseDate = new DateTime(2023, 7, 21),
                    TotaRevenue = 1440000000
                },
                new Movie
                {
                    Id = 4,
                    Name = "Toy Story",
                    DirectorId = 4,
                    ReleaseDate = new DateTime(1995, 11, 22),
                    TotaRevenue = 394436586
                }
            );

            modelBuilder.Entity<MovieGenre>().HasData(
                new MovieGenre { MovieId = 1, GenreId = 1 }, // Inception - Action
                new MovieGenre { MovieId = 1, GenreId = 4 }, // Inception - Sci-Fi
                new MovieGenre { MovieId = 2, GenreId = 2 }, // Pulp Fiction - Drama
                new MovieGenre { MovieId = 3, GenreId = 3 }, // Barbie - Comedy
                new MovieGenre { MovieId = 3, GenreId = 2 }, // Barbie - Drama
                new MovieGenre { MovieId = 4, GenreId = 5 }  // Toy Story - Child
            );

        }
    }
}