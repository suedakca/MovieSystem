using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;

namespace Movies.APP.Features.Movies
{
    public class MovieDeleteRequest : Request, IRequest<CommandResponse>
    {
    }
   
    public class MovieDeleteHandler : Service<Movie>, IRequestHandler<MovieDeleteRequest, CommandResponse>
    {
        public MovieDeleteHandler(DbContext context) : base(context)
        {
        }
        
        protected override IQueryable<Movie> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking).Include(m => m.MovieGenres);
        }
        
        public async Task<CommandResponse> Handle(MovieDeleteRequest request, CancellationToken cancellationToken)
        {
            var entity = await Query(false).SingleOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
            
            if(entity is null)
                Error("Movie not found");
            Delete(entity.MovieGenres);
            Delete(entity);
            return Success("User deleted successfully.", entity.Id);
        }
    }
}