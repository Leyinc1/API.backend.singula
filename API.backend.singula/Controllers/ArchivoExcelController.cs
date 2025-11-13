using Microsoft.AspNetCore.Mvc;
using Singula.Core.Core.DTOs.ArchivoExcel;
using Singula.Core.Core.Interfaces.Services;

namespace API.backend.singula.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArchivoExcelController : ControllerBase
{
    private readonly IArchivoExcelService _service;

    public ArchivoExcelController(IArchivoExcelService service)
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
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string usuario)
    {
        if (file == null || file.Length == 0) return BadRequest("Archivo inválido");
        var buffer = new byte[file.Length];
        await file.OpenReadStream().ReadAsync(buffer);
        var uploadDto = new ArchivoExcelUploadDTO { Contenido = buffer, NombreArchivo = file.FileName, UsuarioCarga = usuario };
        // Guardado temporal en disco
        var tempPath = Path.Combine(Path.GetTempPath(), uploadDto.NombreArchivo);
        await System.IO.File.WriteAllBytesAsync(tempPath, uploadDto.Contenido);
        var created = await _service.CreateAsync(new CreateArchivoExcelDTO { NombreArchivo = uploadDto.NombreArchivo, TamanoBytes = uploadDto.Contenido.Length, UsuarioCarga = uploadDto.UsuarioCarga });
        if (!created.Success) return BadRequest(created);
        // Procesar archivo (se usa la ruta temporal)
        await _service.ProcesarArchivoAsync(tempPath, created.Data!.Id);
        return CreatedAtAction(nameof(Get), new { id = created.Data!.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateArchivoExcelDTO dto)
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
}
