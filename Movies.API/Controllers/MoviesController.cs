using CORE.APP.Models;
using Movies.APP.Features.Movies;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

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

        // =====================================================
        // ================= ADMIN ENDPOINTS ===================
        // =====================================================

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var response = await _mediator.Send(new MovieQueryRequest());
                var list = await response.ToListAsync();
                return list.Any() ? Ok(list) : NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MoviesGet Exception");
                return StatusCode(500, new CommandResponse(false, "MoviesGet failed"));
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var response = await _mediator.Send(new MovieQueryRequest());
                var item = await response.SingleOrDefaultAsync(x => x.Id == id);
                return item != null ? Ok(item) : NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MoviesGetById Exception");
                return StatusCode(500, new CommandResponse(false, "MoviesGetById failed"));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post(MovieCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _mediator.Send(request);
            return response.IsSuccessful ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(MovieUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _mediator.Send(request);
            return response.IsSuccessful ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _mediator.Send(new MovieDeleteRequest { Id = id });
            return response.IsSuccessful ? Ok(response) : BadRequest(response);
        }

        // =====================================================
        // ============== CUSTOMER ENDPOINTS ===================
        // =====================================================

        /// Customer movie list filtered by age (child/adult)
        /// Customer movie list filtered by age (child/adult)
        [HttpGet("customer")]
        [Authorize(Roles = "User,Customer")]
        public async Task<IActionResult> GetForCustomer()
        {
            try
            {
                // JWT içindeki group claim (Child / Adult)
                var group = User.Claims.FirstOrDefault(c => c.Type == "group")?.Value;

                var isChildCustomer = group == "Child";

                var query = new MovieQueryRequest
                {
                    IsChildCustomer = isChildCustomer
                };

                var response = await _mediator.Send(query);
                var list = await response.ToListAsync();

                return list.Any() ? Ok(list) : NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CustomerMovies Exception");
                return StatusCode(500, new CommandResponse(false, "CustomerMovies failed"));
            }
        }
    }
}