using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using smartoffice_web.WebApi.Models;
using smartoffice_web.WebApi.Repositories;

namespace smartoffice_web.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorldController : ControllerBase
    {
        private readonly IWorldRepository _worldRepository;
        private readonly ILogger<WorldController> _logger;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorProvider;

        public WorldController(
            IWorldRepository worldRepository, 
            ILogger<WorldController> logger,
            IActionDescriptorCollectionProvider actionDescriptorProvider)
        {
            _worldRepository = worldRepository;
            _logger = logger;
            _actionDescriptorProvider = actionDescriptorProvider;
        }

        [HttpGet]
        public async Task<IEnumerable<World>> Get()
        {
            return await _worldRepository.GetAllWorldsAsync();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<World>> GetById(Guid id)
        {
            var world = await _worldRepository.GetWorldByIdAsync(id);
            if (world == null)
            {
                return NotFound();
            }
            return Ok(world);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] World world)
        {
            if (world == null)
            {
                return BadRequest("Invalid world data.");
            }

            world.Id = world.Id == Guid.Empty ? Guid.NewGuid() : world.Id; // Ensure an ID is assigned
            await _worldRepository.AddWorldAsync(world);
            
            return CreatedAtAction(nameof(GetById), new { id = world.Id }, world);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] World world)
        {
            if (world == null || id != world.Id)
            {
                return BadRequest("Invalid world data.");
            }

            await _worldRepository.UpdateWorldAsync(world);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _worldRepository.DeleteWorldAsync(id);
            return NoContent();
        }

        // ✅ Route to Get All Available API Endpoints for Debugging
        [HttpGet("routes")]
        public IActionResult GetAllRoutes()
        {
            var routes = _actionDescriptorProvider.ActionDescriptors.Items
                .Where(a => a.AttributeRouteInfo != null)
                .Select(a => new
                {
                    Method = string.Join(", ", a.ActionConstraints?.SelectMany(ac => ac.ToString().Split(' ')) ?? new[] { "ANY" }),
                    Route = a.AttributeRouteInfo?.Template ?? $"{a.RouteValues["controller"]}/{a.RouteValues["action"]}"
                })
                .ToList();

            return Ok(routes);
        }
    }
}
