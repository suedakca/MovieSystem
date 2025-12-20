using CORE.APP.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Movies.APP.Features.Movies;

namespace Movies.API.Controllers;

[Route("api/customer/movies")]
[ApiController]
[Authorize(Roles = "User")]
public class CustomerMoviesController : ControllerBase
{
    private readonly ILogger<CustomerMoviesController> _logger;
    private readonly IMediator _mediator;

    public CustomerMoviesController(ILogger<CustomerMoviesController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Returns the movie list filtered by the authenticated user's group.
    /// Child users only see "Child" genre movies, adults see all other genres.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<CustomerMovieResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Get()
    {
        try
        {
            var isChildGroup = IsChildGroup();
            var movies = await _mediator.Send(new CustomerMoviesQuery { IsChildGroup = isChildGroup });

            if (movies.Any())
                return Ok(movies);

            return NoContent();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "CustomerMoviesGet Exception");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new CommandResponse(false, "An exception occured during CustomerMoviesGet.")
            );
        }
    }

    /// <summary>
    /// Returns movie details (movie + director) according to the authenticated user's group.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CustomerMovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            var isChildGroup = IsChildGroup();
            var movies = await _mediator.Send(new CustomerMoviesQuery
            {
                IsChildGroup = isChildGroup,
                MovieId = id
            });

            var movie = movies.SingleOrDefault();
            if (movie is not null)
                return Ok(movie);

            return NoContent();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "CustomerMoviesGetById Exception");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new CommandResponse(false, "An exception occured during CustomerMoviesGetById.")
            );
        }
    }

    private bool IsChildGroup()
    {
        var groupClaim = User.Claims.FirstOrDefault(c => c.Type == "group");
        return string.Equals(groupClaim?.Value, "Child", StringComparison.OrdinalIgnoreCase);
    }
}
