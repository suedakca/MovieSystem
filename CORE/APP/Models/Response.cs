namespace CORE.APP.Models;

public abstract class Response
{
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the string unique identifier of the response.
    /// Defined as virtual to allow overriding in derived classes.
    /// </summary>
    public virtual string Guid { get; set; }

    /// <summary>
    /// Constructor with parameter to set the Id from a sub (child) class
    /// constructor using Constructor Chaining.
    /// </summary>
    /// <param name="id">The integer unique identifier parameter.</param>
    protected Response(int id)
    {
        Id = id;
    }

    /// <summary>
    /// Default constructor (constructor without any parameters)
    /// that will set the Id to the integer default value (0).
    /// </summary>
    protected Response()
    {
    }
}