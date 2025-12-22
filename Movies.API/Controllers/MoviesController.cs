using CORE.APP.Models;
using Movies.APP.Features.Movies;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Movies.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ILogger<MoviesController> _logger;
        private readonly IMediator _mediator;

        public MoviesController(ILogger<MoviesController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        // GET: api/Movies
        [HttpGet]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var list = await _mediator.Send(new MovieQueryRequest());
                if (list.Any())
                    return Ok(list);

                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "MoviesGet Exception");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new CommandResponse(false, "An exception occured during MoviesGet.")
                );
            }
        }

        // GET: api/Movies/5
        [HttpGet("{id:int}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var list = await _mediator.Send(new MovieQueryRequest());

                // Artık SingleOrDefaultAsync YOK
                var item = list.SingleOrDefault(r => r.Id == id);

                if (item is not null)
                    return Ok(item);

                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "MoviesGetById Exception");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new CommandResponse(false, "An exception occured during MoviesGetById.")
                );
            }
        }

        // POST: api/Movies
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post(MovieCreateRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _mediator.Send(request);

                    if (response.IsSuccessful)
                        return Ok(response);

                    ModelState.AddModelError("MoviesPost", response.Message);
                }

                return BadRequest(
                    new CommandResponse(
                        false,
                        string.Join("|",
                            ModelState.Values
                                .SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage))
                    )
                );
            }
            catch (Exception exception)
            {
                _logger.LogError("MoviesPost Exception: " + exception.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new CommandResponse(false, "An exception occured during MoviesPost.")
                );
            }
        }

        // PUT: api/Movies
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(MovieUpdateRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _mediator.Send(request);

                    if (response.IsSuccessful)
                        return Ok(response);

                    ModelState.AddModelError("MoviesPut", response.Message);
                }

                return BadRequest(
                    new CommandResponse(
                        false,
                        string.Join("|",
                            ModelState.Values
                                .SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage))
                    )
                );
            }
            catch (Exception exception)
            {
                _logger.LogError("MoviesPut Exception: " + exception.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new CommandResponse(false, "An exception occured during MoviesPut.")
                );
            }
        }

        // DELETE: api/Movies/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _mediator.Send(new MovieDeleteRequest { Id = id });

                if (response.IsSuccessful)
                    return Ok(response);

                ModelState.AddModelError("MoviesDelete", response.Message);

                return BadRequest(
                    new CommandResponse(
                        false,
                        string.Join("|",
                            ModelState.Values
                                .SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage))
                    )
                );
            }
            catch (Exception exception)
            {
                _logger.LogError("MoviesDelete Exception: " + exception.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new CommandResponse(false, "An exception occured during MoviesDelete.")
                );
            }
        }
    }
}
