using CORE.APP.Domain;

namespace Movies.APP.Domain
{
    public class Director : Entity
    {
        public int DirectorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsRetired { get; set; }
        public List<Movie> Movies { get; set; }
    }
}