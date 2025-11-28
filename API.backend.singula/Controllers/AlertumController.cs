using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Singula.Core.Services;
using Singula.Core.Services.Dto;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.backend.singula.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Restaurado para producción
    public class AlertumController : ControllerBase
    {
        private readonly IAlertumService _service;

        public AlertumController(IAlertumService service)
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
        public async Task<IActionResult> Create(AlertumDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.IdAlerta }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AlertumDto dto)
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

        // New endpoints ----------------------------------------------------
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId, [FromQuery] bool onlyUnread = false, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var list = await _service.GetByUserAsync(userId, onlyUnread, page, pageSize);
            return Ok(list);
        }

        [HttpGet("user/{userId}/unread/count")]
        public async Task<IActionResult> GetUnreadCount(int userId)
        {
            var count = await _service.GetUnreadCountByUserAsync(userId);
            return Ok(new { Unread = count });
        }

        [HttpPost("{id}/mark-read")]
        public async Task<IActionResult> MarkAsRead(int id, [FromQuery] int userId)
        {
            var ok = await _service.MarkAsReadAsync(id, userId);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
