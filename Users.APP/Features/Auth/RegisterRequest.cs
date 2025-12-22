using CORE.APP.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;
using Users.APP.Domain;

namespace Users.APP.Features.Auth
{
    public class RegisterRequest : IRequest<RegisterResponse>
    {
        [Required, StringLength(30)]
        public string UserName { get; set; }

        [Required, StringLength(15)]
        public string Password { get; set; }
        
        [Required]
        public DateOnly BirthDate { get; set; }

        [Required]
        public Genders Gender { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(250)]
        public string Address { get; set; }
    }
}