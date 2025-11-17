using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace Movie.APP.Domain;

public class MovieDbFactory : IDesignTimeDbContextFactory<MovieDb>
{
    private const string CONNECTIONSTRING = "data source=UsersDb";

    public MovieDb CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MovieDb>();
        optionsBuilder.UseSqlite(CONNECTIONSTRING);
        return new MovieDb(optionsBuilder.Options);
    }
}