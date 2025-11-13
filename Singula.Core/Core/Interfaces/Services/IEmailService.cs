namespace Singula.Core.Core.Interfaces.Services;

/// <summary>
/// Servicio para envío de correos electrónicos
/// </summary>
public interface IEmailService
{
    Task<bool> EnviarAlertaAsync(string destinatario, string asunto, string mensaje);
    Task<bool> EnviarReporteAsync(string destinatario, string asunto, byte[] pdfBytes, string nombreArchivo);
    Task<bool> EnviarNotificacionAsync(string destinatario, string asunto, string cuerpoHtml);
}
