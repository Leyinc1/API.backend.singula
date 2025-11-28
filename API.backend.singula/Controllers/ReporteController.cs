using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Singula.Core.Services;
using Singula.Core.Services.Dto;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Linq;

namespace API.backend.singula.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] // Comentado temporalmente para desarrollo
    public class ReporteController : ControllerBase
    {
        private readonly IReporteService _service;
        private readonly IWebHostEnvironment _env;
        private readonly IDashboardService _dashboardService;

        public ReporteController(IReporteService service, IWebHostEnvironment env, IDashboardService dashboardService)
        {
            _service = service;
            _env = env;
            _dashboardService = dashboardService;
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
            try
            {
                // Logging para debug
                Console.WriteLine($"[ReporteController] Create - GeneradoPor: {dto.GeneradoPor}, TipoReporte: {dto.TipoReporte}");
                Console.WriteLine($"[ReporteController] FiltrosJson: {dto.FiltrosJson}");
                Console.WriteLine($"[ReporteController] NombreArchivo: {dto.NombreArchivo}, RutaArchivo: {dto.RutaArchivo}");
                
                // Validar que generadoPor sea válido
                if (dto.GeneradoPor <= 0)
                {
                    return BadRequest(new { message = "El campo GeneradoPor debe ser un ID de usuario válido" });
                }

                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(Get), new { id = created.IdReporte }, created);
            }
            catch (Exception ex)
            {
                // Loggear excepción completa en consola
                Console.WriteLine($"[ReporteController] ERROR: {ex.Message}");
                Console.WriteLine($"[ReporteController] InnerException: {ex.InnerException?.Message}");
                Console.WriteLine($"[ReporteController] StackTrace: {ex.StackTrace}");
                
                // Retornar mensaje de error detallado
                var innerMsg = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { message = $"Error al crear el reporte: {innerMsg}" });
            }
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
        /// Endpoint específico para obtener datos de dashboard para reportes.
        /// Filtra por fecha de INGRESO (no por fecha de solicitud) y por tipo de solicitud.
        /// Query params: startDate, endDate, tipoSolicitud (ej: "SLA1", "SLA2")
        /// </summary>
        [HttpGet("dashboard-data")]
        public async Task<IActionResult> GetDashboardData(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? tipoSolicitud = null)
        {
            try
            {
                Console.WriteLine($"[ReporteController] GetDashboardData - Start: {startDate}, End: {endDate}, Tipo: {tipoSolicitud}");
                
                // Obtener TODOS los datos sin filtro de bloqueTech (se filtra en frontend)
                var allData = await _dashboardService.GetDashboardDataAsync(
                    startDate: null,  // NO filtrar por fecha en DashboardService
                    endDate: null,    // Filtraremos manualmente después
                    bloqueTech: null,
                    tipoSolicitud: tipoSolicitud,
                    prioridad: null,
                    cumpleSla: null
                );

                Console.WriteLine($"[ReporteController] Total datos del servicio: {allData.Count()}");

                // Ajustar endDate para incluir TODO el día (hasta 23:59:59)
                DateTime? adjustedEndDate = endDate.HasValue 
                    ? endDate.Value.Date.AddDays(1).AddTicks(-1) 
                    : null;

                Console.WriteLine($"[ReporteController] Adjusted End Date: {adjustedEndDate}");

                // Filtrar por rango de fechas usando FechaIngreso (fecha de finalización)
                var filteredData = allData.Where(s => {
                    // Si no hay FechaIngreso, usar FechaSolicitud como fallback
                    var fechaParaComparar = s.FechaIngreso ?? s.FechaSolicitud;
                    
                    if (!fechaParaComparar.HasValue) return false;
                    
                    bool cumpleFechaInicio = !startDate.HasValue || fechaParaComparar.Value.Date >= startDate.Value.Date;
                    bool cumpleFechaFin = !adjustedEndDate.HasValue || fechaParaComparar.Value <= adjustedEndDate.Value;
                    
                    return cumpleFechaInicio && cumpleFechaFin;
                }).ToList();

                // Log para debug
                Console.WriteLine($"[ReporteController] DashboardData - Total: {allData.Count()}, Filtrado: {filteredData.Count}");
                Console.WriteLine($"[ReporteController] Filtros - Start: {startDate}, End: {endDate}, Tipo: {tipoSolicitud}");

                return Ok(filteredData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReporteController] ERROR en GetDashboardData: {ex.Message}");
                return StatusCode(500, new { message = $"Error al obtener datos: {ex.Message}" });
            }
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
