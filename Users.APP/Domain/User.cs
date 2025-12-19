using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Users.APP.Domain
{
    public class User : Entity
    {
        [Required, StringLength(30)]
        public string UserName { get; set; }

        [Required, StringLength(15)]
        public string Password { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        public Genders Gender { get; set; }
        public DateOnly? BirthDate { get; set; }

        public DateTime RegistrationDate { get; set; }
        public decimal Score { get; set; }
        public bool IsActive { get; set; }

        public string Address { get; set; }

        public int? GroupId { get; set; }
        public Group Group { get; set; }

        public List<UserRole> UserRoles { get; set; } = new();

        [NotMapped]
        public List<int> RoleIds
        {
            get => UserRoles.Select(x => x.RoleId).ToList();
            set => UserRoles = value.Select(id => new UserRole { RoleId = id }).ToList();
        }

        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
    }
}