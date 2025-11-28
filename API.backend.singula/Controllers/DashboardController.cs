using Microsoft.AspNetCore.Mvc;
using Singula.Core.Services;
using System;
using System.Threading.Tasks;

namespace API.backend.singula.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;

        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtiene datos de SLA para el dashboard con filtros opcionales
        /// </summary>
        [HttpGet("sla/data")]
        public async Task<IActionResult> GetSlaData(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? bloqueTech,
            [FromQuery] string? tipoSolicitud,
            [FromQuery] string? prioridad,
            [FromQuery] bool? cumpleSla)
        {
            try
            {
                var data = await _service.GetDashboardDataAsync(
                    startDate, endDate, bloqueTech, tipoSolicitud, prioridad, cumpleSla);
                
                return Ok(new { data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al obtener datos del dashboard", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene estadísticas generales de SLA
        /// </summary>
        [HttpGet("sla/statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var statistics = await _service.GetStatisticsAsync();
                return Ok(new { data = statistics });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al obtener estadísticas", message = ex.Message });
            }
        }

        [HttpGet("filters")]
        public async Task<IActionResult> GetAvailableFilters()
        {
            try
            {
                var filters = await _service.GetAvailableFiltersAsync();
                return Ok(new { data = filters });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al obtener filtros disponibles", message = ex.Message });
            }
        }
    }
}
