using Microsoft.Extensions.Logging;
using Singula.Core.Core.Interfaces.Services;

namespace Singula.Core.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task<bool> EnviarAlertaAsync(string destinatario, string asunto, string mensaje)
    {
        _logger.LogInformation("[EmailService] Enviando alerta a {destinatario}: {asunto}", destinatario, asunto);
        // Implementación mock / placeholder - integrar SMTP o provider real
        return Task.FromResult(true);
    }

    public Task<bool> EnviarReporteAsync(string destinatario, string asunto, byte[] pdfBytes, string nombreArchivo)
    {
        _logger.LogInformation("[EmailService] Enviando reporte a {destinatario}: {nombreArchivo} ({bytes} bytes)", destinatario, nombreArchivo, pdfBytes.Length);
        return Task.FromResult(true);
    }

    public Task<bool> EnviarNotificacionAsync(string destinatario, string asunto, string cuerpoHtml)
    {
        _logger.LogInformation("[EmailService] Enviando notificación a {destinatario}: {asunto}", destinatario, asunto);
        return Task.FromResult(true);
    }
}
