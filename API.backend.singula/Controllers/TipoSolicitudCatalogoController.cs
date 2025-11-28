using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Singula.Core.Services;
using Singula.Core.Services.Dto;
using System.Threading.Tasks;

namespace API.backend.singula.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] // Deshabilitado temporalmente - configuración no requiere autenticación
    public class TipoSolicitudCatalogoController : ControllerBase
    {
        private readonly ITipoSolicitudCatalogoService _service;

        public TipoSolicitudCatalogoController(ITipoSolicitudCatalogoService service)
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
        public async Task<IActionResult> Create(TipoSolicitudCatalogoDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.IdTipoSolicitud }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TipoSolicitudCatalogoDto dto)
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
                // Error de foreign key constraint
                return BadRequest(new 
                { 
                    error = "FOREIGN_KEY_CONSTRAINT",
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                // Otros errores
                return StatusCode(500, new 
                { 
                    error = "INTERNAL_ERROR",
                    message = "Error al eliminar el tipo de solicitud: " + ex.Message
                });
            }
        }
    }
}
