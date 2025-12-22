using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Users.APP.Features.Groups;

namespace Users.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]     public class GroupsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public GroupsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Send a GroupQueryRequest to MediatR, which dispatches it to the appropriate handler (GroupQueryHandler).
            var query = await _mediator.Send(new GroupQueryRequest());

            // Execute the query and retrieve the results as a list asynchronously.
            var list = await query.ToListAsync();

            return Ok(list);
        }
        
        [HttpGet("{id}")] // get route: /Groups/5 (name defined in {} must be same as the action's parameter name, id will be 5)
        public async Task<IActionResult> Get(int id)
        {
            var query = await _mediator.Send(new GroupQueryRequest());
            
            var item = await query.SingleOrDefaultAsync(groupResponse => groupResponse.Id == id);

            if (item is null)
                return NotFound();

            return Ok(item);
        }
        
        [HttpPost] // post route: /Groups
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Post(GroupCreateRequest request)
        {
            // Check if the incoming request model passes validations through data annotations.
            if (ModelState.IsValid)
            {
                // Send the creation request to MediatR, which will route it to the appropriate handler (GroupCreateHandler).
                var response = await _mediator.Send(request);

                // If the group was created successfully
                if (response.IsSuccessful)
                {
                    return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
                }

                return BadRequest(response);
            }
            
            return BadRequest(ModelState);
        }
        
        [HttpPut] // put route: /Groups
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Put(GroupUpdateRequest request)
        {
            // Check if the incoming request model passes validations through data annotations.
            if (ModelState.IsValid)
            {
                // Send the update request to MediatR, which will route it to the appropriate handler (GroupUpdateHandler).
                var response = await _mediator.Send(request);

                // If the group was updated successfully
                if (response.IsSuccessful)
                {
                    return NoContent();
                }

                return BadRequest(response);
            }

            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _mediator.Send(new GroupDeleteRequest() { Id = id });

            // If group was deleted successfully
            if (response.IsSuccessful)
            {
                return NoContent(); 
            }

            return BadRequest(response);
        }
    }
}