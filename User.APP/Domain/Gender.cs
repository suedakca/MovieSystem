using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;


namespace User.APP.Domain
{
    public class Gender : Entity
    {

        
        [Required]
        [StringLength(100)] 
        public string Title { get; set; }



        // for group-users one to many relationship
        public List<User> Users { get; set; } = new List<User>(); 
    }
}