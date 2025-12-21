using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Movies.APP.Domain;

namespace Movies.APP.Features.Directors
{
    // request properties are created according to the data that will be retrieved from APIs or UIs
    public class DirectorCreateRequest : IRequest<CommandResponse>
    {
        // copy all the non navigation properties from Director entity
        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        public string LastName { get; set; }

        public bool IsRetired { get; set; }
    }

    public class DirectorCreateHandler : Service<Director>, IRequestHandler<DirectorCreateRequest, CommandResponse>
    {
        public DirectorCreateHandler(DbContext db) : base(db)
        {
        }

        public async Task<CommandResponse> Handle(DirectorCreateRequest request, CancellationToken cancellationToken)
        {
            // d: Director entity delegate. Check if a director with the same first name and last name exists.
            if (await Query().AnyAsync(d =>
                        d.FirstName == request.FirstName.Trim() &&
                        d.LastName == request.LastName.Trim(),
                    cancellationToken))
            {
                return Error("Director with the same name exists!");
            }

            var entity = new Director
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                IsRetired = request.IsRetired
            };

            Create(entity);

            return Success("Director created successfully.", entity.Id);
        }
    }
}