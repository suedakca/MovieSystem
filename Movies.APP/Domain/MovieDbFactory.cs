using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Movies.APP.Domain
{
    public class MovieDbFactory : IDesignTimeDbContextFactory<MovieDb>
    {
        private const string ConnectionString = "data source=MovieDB";

        public MovieDb CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MovieDb>();
            optionsBuilder.UseSqlite(ConnectionString);
            return new MovieDb(optionsBuilder.Options);
        }
    }
}