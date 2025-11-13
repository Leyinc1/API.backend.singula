using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Singula.Core.Core.Interfaces.Services;
using Singula.Core.Core.Settings;

namespace Singula.Core.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly EmailSettings _settings;

    public EmailService(ILogger<EmailService> logger, EmailSettings settings)
    {
        _logger = logger;
        _settings = settings;
    }

    public async Task<bool> EnviarAlertaAsync(string destinatario, string asunto, string mensaje)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_settings.SmtpServer))
            {
                _logger.LogWarning("SMTP no configurado, simulando envío");
                return true;
            }

            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            msg.To.Add(MailboxAddress.Parse(destinatario));
            msg.Subject = asunto;
            msg.Body = new TextPart("plain") { Text = mensaje };

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, _settings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
            if (!string.IsNullOrEmpty(_settings.Username))
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
            await client.SendAsync(msg);
            await client.DisconnectAsync(true);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando email");
            return false;
        }
    }

    public async Task<bool> EnviarReporteAsync(string destinatario, string asunto, byte[] pdfBytes, string nombreArchivo)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_settings.SmtpServer))
            {
                _logger.LogWarning("SMTP no configurado, simulando envío");
                return true;
            }

            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            msg.To.Add(MailboxAddress.Parse(destinatario));
            msg.Subject = asunto;

            var builder = new BodyBuilder { TextBody = "Adjunto encontrará el reporte." };
            builder.Attachments.Add(nombreArchivo, pdfBytes, new ContentType("application", "pdf"));
            msg.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, _settings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
            if (!string.IsNullOrEmpty(_settings.Username))
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
            await client.SendAsync(msg);
            await client.DisconnectAsync(true);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando email con adjunto");
            return false;
        }
    }

    public Task<bool> EnviarNotificacionAsync(string destinatario, string asunto, string cuerpoHtml)
    {
        // Simple wrapper
        var msg = new MimeMessage();
        msg.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        msg.To.Add(MailboxAddress.Parse(destinatario));
        msg.Subject = asunto;
        msg.Body = new TextPart("html") { Text = cuerpoHtml };
        _logger.LogInformation("Notificación preparada para {dest}", destinatario);
        return Task.FromResult(true);
    }
}
