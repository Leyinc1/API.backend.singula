using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Singula.Core.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API.backend.singula.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] // keep commented for development
    public class SlaController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<SlaController> _logger;
        private readonly IDashboardService _dashboardService;

        // Extensiones permitidas
        private static readonly string[] AllowedExtensions = new[] { ".xlsx", ".xls" };
        private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

        public SlaController(IWebHostEnvironment env, ILogger<SlaController> logger, IDashboardService dashboardService)
        {
            _env = env;
            _logger = logger;
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Upload an Excel file related to SLA (multipart/form-data)
        /// Saves file under wwwroot/uploads/sla or ./Uploads/sla if wwwroot not present.
        /// </summary>
        [HttpPost("upload")]
        [RequestSizeLimit(50_000_000)] // allow up to ~50MB
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file provided" });

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                return BadRequest(new { error = "Invalid file extension. Only .xlsx and .xls allowed." });

            if (file.Length > MaxFileSize)
                return BadRequest(new { error = $"File too large. Max allowed is {MaxFileSize / (1024 * 1024)} MB." });

            try
            {
                var uploadsRoot = Path.Combine(_env.ContentRootPath, "wwwroot");
                if (!Directory.Exists(uploadsRoot))
                {
                    // fallback to project-level Uploads folder
                    uploadsRoot = Path.Combine(_env.ContentRootPath, "Uploads");
                }

                var slaFolder = Path.Combine(uploadsRoot, "uploads", "sla");
                Directory.CreateDirectory(slaFolder);

                var safeFileName = Path.GetFileName(file.FileName);
                var unique = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid():N}_{safeFileName}";
                var filePath = Path.Combine(slaFolder, unique);

                await using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                // Return relative path from content root so caller can reference
                var relativePath = Path.GetRelativePath(_env.ContentRootPath, filePath);

                return Ok(new { fileName = unique, path = relativePath });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving SLA file");
                return StatusCode(500, new { error = "Error saving file", details = ex.Message });
            }
        }

        /// <summary>
        /// Get SLA dashboard data (GET /api/sla/data)
        /// Optional query params: startDate, endDate, bloqueTech, tipoSolicitud, prioridad, cumpleSla
        /// </summary>
        [HttpGet("data")]
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
                var data = await _dashboardService.GetDashboardDataAsync(
                    startDate, endDate, bloqueTech, tipoSolicitud, prioridad, cumpleSla);
                return Ok(new { data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting SLA dashboard data");
                return StatusCode(500, new { error = "Error getting SLA dashboard data", details = ex.Message });
            }
        }
    }
}
