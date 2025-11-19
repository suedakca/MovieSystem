using Microsoft.EntityFrameworkCore;

namespace Movies.APP.Domain
{

    public class MovieDb : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Director> Directors { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }

        public MovieDb(DbContextOptions<MovieDb> options) : base(options)
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
        }
    }
}