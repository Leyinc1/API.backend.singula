using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Singula.Core.Services;
using Singula.Core.Services.Dto;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;

namespace API.backend.singula.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReporteController : ControllerBase
    {
        private readonly IReporteService _service;
        private readonly IWebHostEnvironment _env;

        public ReporteController(IReporteService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
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

        /// <summary>
        /// Endpoint para subir un archivo (PDF) y devolver la ruta relativa donde se guardó.
        /// Guarda en wwwroot/reports y devuelve { ruta: "/reports/archivo.pdf" }
        /// </summary>
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se recibió archivo");

            try
            {
                var webRoot = _env.WebRootPath;
                if (string.IsNullOrEmpty(webRoot))
                {
                    webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }

                var reportsDir = Path.Combine(webRoot, "reports");
                if (!Directory.Exists(reportsDir)) Directory.CreateDirectory(reportsDir);

                var safeFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                var fullPath = Path.Combine(reportsDir, safeFileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativePath = $"/reports/{safeFileName}";
                return Ok(new { ruta = relativePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
