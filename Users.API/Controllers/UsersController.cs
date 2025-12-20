#nullable disable
using CORE.APP.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Users.APP.Features.Users;

//Generated from Custom Microservices Template.
namespace Users.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IMediator _mediator;

        public UsersController(ILogger<UsersController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        // =====================================================
        // ===================== ADMIN =========================
        // =====================================================

        // GET: api/Users
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            try
            {
                var response = await _mediator.Send(new UserQueryRequest());
                var list = await response.ToListAsync();
                return list.Any() ? Ok(list) : NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError("UsersGet Exception: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new CommandResponse(false, "An exception occured during UsersGet."));
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var response = await _mediator.Send(new UserQueryRequest());
                var item = await response.SingleOrDefaultAsync(r => r.Id == id);
                return item != null ? Ok(item) : NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError("UsersGetById Exception: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new CommandResponse(false, "An exception occured during UsersGetById."));
            }
        }

        // PUT: api/Users
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(UserUpdateRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _mediator.Send(request);
                    if (response.IsSuccessful)
                        return Ok(response);

                    ModelState.AddModelError("UsersPut", response.Message);
                }

                return BadRequest(new CommandResponse(false,
                    string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            catch (Exception ex)
            {
                _logger.LogError("UsersPut Exception: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new CommandResponse(false, "An exception occured during UsersPut."));
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _mediator.Send(new UserDeleteRequest { Id = id });
                return response.IsSuccessful ? Ok(response) : BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("UsersDelete Exception: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new CommandResponse(false, "An exception occured during UsersDelete."));
            }
        }

        // POST: api/Users/GetFiltered
        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> GetFiltered(UserQueryRequest request)
        {
            var response = await _mediator.Send(request);
            var list = await response.ToListAsync();
            return list.Any() ? Ok(list) : NoContent();
        }

        // =====================================================
        // =================== CUSTOMER ========================
        // =====================================================

        /// <summary>
        /// Customer rates a movie (only once).
        /// Score +10 after successful rating.
        /// </summary>
        [HttpPost("rate-movie")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> RateMovie(UserRateMovieRequest request)
        {
            try
            {
                // JWT'den UserId al
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized();

                request.UserId = int.Parse(userIdClaim.Value);

                var response = await _mediator.Send(request);
                return response.IsSuccessful ? Ok(response) : BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("RateMovie Exception: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new CommandResponse(false, "An exception occured during RateMovie."));
            }
        }
    }
}