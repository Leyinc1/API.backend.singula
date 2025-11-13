using Microsoft.AspNetCore.Mvc;
using Singula.Core.Core.DTOs.Alerta;
using Singula.Core.Core.Interfaces.Services;

namespace API.backend.singula.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertaController : ControllerBase
{
    private readonly IAlertaService _service;

    public AlertaController(IAlertaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var resp = await _service.GetAllAsync(new Singula.Core.Core.DTOs.Common.PaginationParams { Page = page, PageSize = pageSize });
        return resp.Success ? Ok(resp) : BadRequest(resp);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var resp = await _service.GetByIdAsync(id);
        return resp.Success ? Ok(resp) : NotFound(resp);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAlertaDTO dto)
    {
        var resp = await _service.CreateAsync(dto);
        return resp.Success ? CreatedAtAction(nameof(Get), new { id = resp.Data!.Id }, resp) : BadRequest(resp);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAlertaDTO dto)
    {
        var resp = await _service.UpdateAsync(id, dto);
        return resp.Success ? Ok(resp) : BadRequest(resp);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var resp = await _service.DeleteAsync(id);
        return resp.Success ? Ok(resp) : NotFound(resp);
    }

    [HttpPost("{id}/send")]
    public async Task<IActionResult> SendByEmail(int id)
    {
        var resp = await _service.EnviarAlertaPorEmailAsync(id);
        return resp.Success ? Ok(resp) : BadRequest(resp);
    }

    [HttpGet("resumen")]
    public async Task<IActionResult> Resumen()
    {
        var resp = await _service.GetResumenAsync();
        return resp.Success ? Ok(resp) : BadRequest(resp);
    }
}
