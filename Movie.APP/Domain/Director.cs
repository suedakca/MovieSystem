namespace Movie.APP.Domain;

public class Director : Entity
{
    public int DirectorId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsRetired { get; set; }
}