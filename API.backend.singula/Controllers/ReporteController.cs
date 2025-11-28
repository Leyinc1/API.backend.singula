using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Singula.Core.Services;
using Singula.Core.Services.Dto;
using System.Threading.Tasks;

namespace API.backend.singula.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] // Comentado temporalmente para desarrollo
    public class ReporteController : ControllerBase
    {
        private readonly IReporteService _service;

        public ReporteController(IReporteService service)
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
        public async Task<IActionResult> Create(ReporteDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.IdReporte }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ReporteDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
