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
    public class Environment2DController : ControllerBase
    {
        private readonly IEnvironment2DRepository _environment2DRepository;
        private readonly ILogger<Environment2DController> _logger;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorProvider;

        public Environment2DController(
            IEnvironment2DRepository environment2DRepository, 
            ILogger<Environment2DController> logger,
            IActionDescriptorCollectionProvider actionDescriptorProvider)
        {
            _environment2DRepository = environment2DRepository;
            _logger = logger;
            _actionDescriptorProvider = actionDescriptorProvider;
        }

        [HttpGet]
        public async Task<IEnumerable<Environment2D>> Get()
        {
            return await _environment2DRepository.GetAllEnvironment2DsAsync();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Environment2D>> GetById(Guid id)
        {
            var world = await _environment2DRepository.GetWorldByIdAsync(id);
            if (world == null)
            {
                return NotFound();
            }
            return Ok(world);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Environment2D environment2D)
        {
            if (environment2D == null)
            {
                return BadRequest("Invalid world data.");
            }

            environment2D.Id = environment2D.Id == Guid.Empty ? Guid.NewGuid() : environment2D.Id; // Ensure an ID is assigned
            await _environment2DRepository.AddWorldAsync(environment2D);
            
            return CreatedAtAction(nameof(GetById), new { id = environment2D.Id }, environment2D);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Environment2D environment2D)
        {
            if (environment2D == null || id != environment2D.Id)
            {
                return BadRequest("Invalid world data.");
            }

            await _environment2DRepository.UpdateWorldAsync(environment2D);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _environment2DRepository.DeleteWorldAsync(id);
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
