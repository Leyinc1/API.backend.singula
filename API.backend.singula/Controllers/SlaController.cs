using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Singula.Core.Services;
using Singula.Core.Services.Dto;
using System;
using System.IO;
using System.Threading.Tasks;

namespace API.backend.singula.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SlaController : ControllerBase
    {
        private readonly ISlaService _slaService;

        public SlaController(ISlaService slaService)
        {
            _slaService = slaService;
        }

        // Endpoint to create a manual entry coming from frontend form
        [HttpPost("manual")]
        public async Task<IActionResult> CreateManualEntry([FromBody] ManualEntryDto dto)
        {
            if (dto == null) return BadRequest("Invalid payload");
            try
            {
                var created = await _slaService.CreateManualEntryAsync(dto);
                return CreatedAtAction("Get", "Solicitud", new { id = created.IdSolicitud }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Endpoint to accept Excel uploads (multipart/form-data)
        [HttpPost("upload-excel")]
        [RequestSizeLimit(20_000_000)]
        public async Task<IActionResult> UploadExcel([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file uploaded");
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var bytes = ms.ToArray();
            var result = await _slaService.SaveUploadedFileAsync(bytes, file.FileName);
            return Ok(result);
        }
    }
}
