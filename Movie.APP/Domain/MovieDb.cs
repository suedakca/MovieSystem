using Microsoft.EntityFrameworkCore;
namespace Movie.APP.Domain;

public class MovieDb(DbContextOptions<MovieDb> options) : DbContext(options)
{
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<MovieGenre> MovieGenres { get; set; }
    public DbSet<Director> Directors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>().HasIndex(movieEntity => movieEntity.Name).IsUnique();
        modelBuilder.Entity<Genre>().HasIndex(genreEntity => genreEntity.Name).IsUnique();
        
        // Composite index on FirstName and LastName for optimizing searches involving both fields.
        modelBuilder.Entity<Director>().HasIndex(directorEntity => new {directorEntity.FirstName, directorEntity.LastName});
    }
}