using Microsoft.AspNetCore.Mvc;
using Singula.Core.Core.DTOs.Prediccion;
using Singula.Core.Core.Interfaces.Services;

namespace API.backend.singula.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrediccionController : ControllerBase
{
    private readonly IPrediccionService _service;

    public PrediccionController(IPrediccionService service)
    {
        _service = service;
    }

    [HttpPost("generar")]
    public async Task<IActionResult> Generar([FromBody] CreatePrediccionDTO dto)
    {
        var resp = await _service.GenerarPrediccionAsync(dto);
        return resp.Success ? Ok(resp) : BadRequest(resp);
    }

    [HttpGet("{indicador}")]
    public async Task<IActionResult> GetByIndicador(string indicador)
    {
        var resp = await _service.GetByIndicadorAsync(indicador);
        return resp.Success ? Ok(resp) : NotFound(resp);
    }
}
