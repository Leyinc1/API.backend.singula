using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Singula.Core.Core.DTOs;
using Singula.Core.Infrastructure.Data;
using Singula.Core.Services;
using Singula.Core.Services.Dto;

namespace API.backend.singula.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] // TODO: Restaurar para producción
    public class AlertumController : ControllerBase
    {
        private readonly IAlertumService _service;
        private readonly ApplicationDbContext _context;

        public AlertumController(IAlertumService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
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

        // ============================================================
        //  ENDPOINTS: FEATURE BACKEND NOTIFICACIONES (DASHBOARD & SLA)
        // ============================================================

        // --- MÉTODO DASHBOARD UNIFICADO Y CORREGIDO ---
        [HttpGet("dashboard-resumen")]
        [AllowAnonymous] // TODO: Cambiar a [Authorize] en producción
        public async Task<ActionResult<IEnumerable<AlertaDashboardDto>>> GetDashboardAlerts()
        {
            // PASO 1: Ejecutar la lógica de negocio (Calcular días y generar alertas nuevas)
            await _service.SincronizarAlertasAutomaticas();

            // PASO 2: Consultar la base de datos para devolver al Frontend
            var alertas = await _context.Alerta
                .Include(a => a.IdSolicitudNavigation)
                    .ThenInclude(s => s.IdRolRegistroNavigation)
                .Include(a => a.IdSolicitudNavigation)
                    .ThenInclude(s => s.IdSlaNavigation) // Importante para fechas
                .Include(a => a.IdTipoAlertaNavigation)
                .Include(a => a.IdEstadoAlertaNavigation)

                // FILTRO: Solo traer las que NO han sido leídas (ID 1)
                .Where(a => a.IdEstadoAlerta == 1)

                .OrderByDescending(a => a.FechaCreacion)
                .Select(a => new AlertaDashboardDto
                {
                    IdAlerta = a.IdAlerta,
                    Mensaje = a.Mensaje,
                    Nivel = a.Nivel,
                    RolAfectado = a.IdSolicitudNavigation.IdRolRegistroNavigation.NombreRol,

                    // Fechas clave
                    FechaSolicitud = a.IdSolicitudNavigation.FechaSolicitud,
                    FechaCreacionSla = a.IdSolicitudNavigation.IdSlaNavigation.CreadoEn, // Dato para cálculo corregido

                    // Datos numéricos
                    DiasTotal = (int)(a.IdSolicitudNavigation.NumDiasSla ?? 0),
                    DiasLimite = a.IdSolicitudNavigation.IdSlaNavigation.DiasUmbral ?? 0,

                    // Tipos e IDs
                    TipoAlerta = a.IdTipoAlertaNavigation.Codigo,
                    IdTipoAlerta = a.IdTipoAlerta,

                    EsNueva = true // Si pasó el filtro Where(IdEstadoAlerta == 1), es nueva.
                })
                .ToListAsync();

            return Ok(alertas);
        }

        [HttpPut("marcar-leida/{id}")]
        [AllowAnonymous] // TODO: Cambiar a [Authorize] en producción
        public async Task<IActionResult> MarcarComoLeida(int id)
        {
            var alerta = await _context.Alerta.FindAsync(id);

            if (alerta == null)
            {
                return NotFound();
            }

            // CAMBIAMOS EL ESTADO A 2 (LEÍDO/ARCHIVADO)
            alerta.IdEstadoAlerta = 2;
            alerta.FechaLectura = DateTime.UtcNow; // Guardamos cuándo se leyó

            await _context.SaveChangesAsync();

            return Ok(new { message = "Alerta marcada como leída" });
        }

        [HttpPut("marcar-todos-incumplimientos")]
        [AllowAnonymous] // TODO: Cambiar a [Authorize] en producción
        public async Task<IActionResult> MarcarTodosIncumplimientos()
        {
            // Buscamos todas las alertas que sean:
            // 1. Estado = 1 (No Leída)
            // 2. Tipo = 2 (Incumplimiento/Rojo)
            var alertas = await _context.Alerta
                .Where(a => a.IdEstadoAlerta == 1 && a.IdTipoAlerta == 2)
                .ToListAsync();

            if (!alertas.Any())
            {
                return Ok(new { message = "No hay incumplimientos pendientes" });
            }

            foreach (var alerta in alertas)
            {
                alerta.IdEstadoAlerta = 2; // Leído
                alerta.FechaLectura = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Incumplimientos marcados como leídos" });
        }

        // ============================================================
        //  ENDPOINTS: MASTER (FILTROS POR USUARIO GENÉRICOS)
        // ============================================================

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

        // Nota: Este endpoint es similar a 'marcar-leida', pero usa el servicio en lugar del contexto directo.
        // Se mantiene para compatibilidad con otras partes del sistema.
        [HttpPost("{id}/mark-read")]
        public async Task<IActionResult> MarkAsRead(int id, [FromQuery] int userId)
        {
            var ok = await _service.MarkAsReadAsync(id, userId);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}