using Microsoft.AspNetCore.Mvc;
using Singula.Core.Core.DTOs.Reporte;
using Singula.Core.Core.Interfaces.Services;

namespace API.backend.singula.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReporteController : ControllerBase
{
    private readonly IReporteService _service;

    public ReporteController(IReporteService service)
    {
        _service = service;
    }

    [HttpPost("generar")]
    public async Task<IActionResult> Generar([FromBody] CreateReporteDTO dto)
    {
        var resp = await _service.GenerarReportePDFAsync(dto);
        if (!resp.Success) return BadRequest(resp);
        return File(resp.Data!.ContenidoPDF, "application/pdf", resp.Data.NombreArchivo);
    }

    [HttpGet("{id}/descargar")]
    public async Task<IActionResult> Descargar(int id)
    {
        var resp = await _service.DescargarReporteAsync(id);
        if (!resp.Success) return NotFound(resp);
        return File(resp.Data!, "application/pdf", $"reporte_{id}.pdf");
    }
}
