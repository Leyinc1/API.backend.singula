using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Singula.Core.Core.Entities;
using Singula.Core.Infrastructure.Data;
using Singula.Core.Services;

namespace API.backend.singula.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificacionEmailController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly EmailService _emailService;

    public NotificacionEmailController(ApplicationDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    // GET: api/NotificacionEmail/{userId}
    [HttpGet("{userId}")]
    [AllowAnonymous]
    public async Task<ActionResult<ConfiguracionNotificacionEmail>> GetConfiguracion(int userId)
    {
        var config = await _context.ConfiguracionNotificacionEmails
            .FirstOrDefaultAsync(c => c.IdUsuario == userId && c.Activo);

        if (config == null)
        {
            // Crear configuración por defecto si no existe
            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            config = new ConfiguracionNotificacionEmail
            {
                IdUsuario = userId,
                Email = usuario.Correo,
                NotificarIncumplimientos = true,
                NotificarPorVencer = true,
                EnviarResumenDiario = false,
                HoraResumenDiario = new TimeSpan(8, 0, 0),
                Activo = true
            };

            _context.ConfiguracionNotificacionEmails.Add(config);
            await _context.SaveChangesAsync();
        }

        return Ok(config);
    }

    // POST: api/NotificacionEmail
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ConfiguracionNotificacionEmail>> CrearConfiguracion(ConfiguracionNotificacionEmailDto dto)
    {
        var existente = await _context.ConfiguracionNotificacionEmails
            .FirstOrDefaultAsync(c => c.IdUsuario == dto.IdUsuario && c.Activo);

        if (existente != null)
        {
            return BadRequest(new { message = "Ya existe una configuración activa para este usuario" });
        }

        var config = new ConfiguracionNotificacionEmail
        {
            IdUsuario = dto.IdUsuario,
            Email = dto.Email,
            NotificarIncumplimientos = dto.NotificarIncumplimientos,
            NotificarPorVencer = dto.NotificarPorVencer,
            EnviarResumenDiario = dto.EnviarResumenDiario,
            HoraResumenDiario = dto.HoraResumenDiario,
            Activo = true
        };

        _context.ConfiguracionNotificacionEmails.Add(config);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetConfiguracion), new { userId = config.IdUsuario }, config);
    }

    // PUT: api/NotificacionEmail/{id}
    [HttpPut("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> ActualizarConfiguracion(int id, ConfiguracionNotificacionEmailDto dto)
    {
        var config = await _context.ConfiguracionNotificacionEmails.FindAsync(id);
        if (config == null)
        {
            return NotFound(new { message = "Configuración no encontrada" });
        }

        config.Email = dto.Email;
        config.NotificarIncumplimientos = dto.NotificarIncumplimientos;
        config.NotificarPorVencer = dto.NotificarPorVencer;
        config.EnviarResumenDiario = dto.EnviarResumenDiario;
        config.HoraResumenDiario = dto.HoraResumenDiario;
        config.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(config);
    }

    // POST: api/NotificacionEmail/test-email
    [HttpPost("test-email")]
    [AllowAnonymous]
    public async Task<IActionResult> EnviarEmailPrueba([FromBody] TestEmailDto dto)
    {
        var alerta = new AlertaEmailDto
        {
            Mensaje = "Esta es una alerta de prueba del sistema Singula",
            Nivel = "Crítico",
            RolAfectado = "Desarrollador Backend",
            DiasTotal = 45,
            DiasLimite = 35,
            TipoAlerta = "TIPO_INCUMPLIMIENTO",
            FechaSolicitud = DateTime.Now.AddDays(-45)
        };

        var resultado = await _emailService.EnviarNotificacionAlertaAsync(
            dto.Email,
            dto.Nombre,
            alerta
        );

        if (resultado)
        {
            return Ok(new { message = "Email de prueba enviado exitosamente" });
        }

        return BadRequest(new { message = "Error al enviar el email de prueba" });
    }

    // POST: api/NotificacionEmail/enviar-resumen
    [HttpPost("enviar-resumen")]
    [AllowAnonymous]
    public async Task<IActionResult> EnviarResumenDiario([FromBody] int userId)
    {
        var config = await _context.ConfiguracionNotificacionEmails
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(c => c.IdUsuario == userId && c.Activo && c.EnviarResumenDiario);

        if (config == null)
        {
            return NotFound(new { message = "Configuración no encontrada o resumen diario desactivado" });
        }

        // Obtener estadísticas de alertas
        var alertas = await _context.Alerta
            .Where(a => a.IdEstadoAlerta == 1) // Solo alertas no leídas
            .ToListAsync();

        var resumen = new ResumenAlertas
        {
            TotalIncumplimientos = alertas.Count(a => a.IdTipoAlerta == 2),
            TotalPorVencer = alertas.Count(a => a.IdTipoAlerta == 1),
            TotalAlertas = alertas.Count
        };

        var resultado = await _emailService.EnviarResumenDiarioAsync(
            config.Email,
            config.Usuario?.Username ?? "Usuario",
            resumen
        );

        if (resultado)
        {
            return Ok(new { message = "Resumen diario enviado exitosamente" });
        }

        return BadRequest(new { message = "Error al enviar el resumen diario" });
    }
}

public class ConfiguracionNotificacionEmailDto
{
    public int IdUsuario { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool NotificarIncumplimientos { get; set; } = true;
    public bool NotificarPorVencer { get; set; } = true;
    public bool EnviarResumenDiario { get; set; } = false;
    public TimeSpan HoraResumenDiario { get; set; } = new TimeSpan(8, 0, 0);
}

public class TestEmailDto
{
    public string Email { get; set; } = string.Empty;
    public string Nombre { get; set; } = "Usuario";
}
