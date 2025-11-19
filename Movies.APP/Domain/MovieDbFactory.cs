using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Movies.APP.Domain
{
    public class MovieDbFactory : IDesignTimeDbContextFactory<MovieDb>
    {
        const string CONNECTIONSTRING = "data source=MovieDB";

        public MovieDb CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MovieDb>();
            optionsBuilder.UseSqlite(CONNECTIONSTRING);
            return new MovieDb(optionsBuilder.Options);
        }
    }
}