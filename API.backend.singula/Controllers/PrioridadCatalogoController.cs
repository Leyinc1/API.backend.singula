using Microsoft.AspNetCore.Mvc;
using Singula.Core.Services;
using Singula.Core.Services.Dto;
using System.Threading.Tasks;
using System;

namespace API.backend.singula.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrioridadCatalogoController : ControllerBase
    {
        private readonly IPrioridadCatalogoService _service;

        public PrioridadCatalogoController(IPrioridadCatalogoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PrioridadCatalogoDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.IdPrioridad }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PrioridadCatalogoDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ok = await _service.DeleteAsync(id);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new 
                { 
                    error = "FOREIGN_KEY_CONSTRAINT",
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    error = "INTERNAL_ERROR",
                    message = "Error al eliminar la prioridad: " + ex.Message
                });
            }
        }
    }
}
