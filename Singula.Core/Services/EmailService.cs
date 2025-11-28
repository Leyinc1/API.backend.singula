using Resend;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Singula.Core.Services;

public class EmailService
{
    private readonly ResendClient _resendClient;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService(IConfiguration configuration)
    {
        var apiKey = configuration["Resend:ApiKey"] ?? throw new ArgumentNullException("Resend:ApiKey");
        _fromEmail = configuration["Resend:FromEmail"] ?? throw new ArgumentNullException("Resend:FromEmail");
        _fromName = configuration["Resend:FromName"] ?? "Sistema Singula";
        
        var httpClient = new HttpClient();
        _resendClient = new ResendClient(new ResendClientOptionsSnapshot(apiKey), httpClient);
    }

    public async Task<bool> EnviarNotificacionAlertaAsync(string destinatario, string nombreDestinatario, AlertaEmailDto alerta)
    {
        try
        {
            var htmlBody = GenerarHtmlAlerta(nombreDestinatario, alerta);
            
            var message = new EmailMessage
            {
                From = _fromEmail,
                To = new[] { destinatario },
                Subject = $"⚠️ {alerta.TipoAlerta}: {alerta.RolAfectado}",
                HtmlBody = htmlBody
            };

            var response = await _resendClient.EmailSendAsync(message);
            return response != null && response.Content != null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al enviar email: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> EnviarResumenDiarioAsync(string destinatario, string nombreDestinatario, ResumenAlertas resumen)
    {
        try
        {
            var htmlBody = GenerarHtmlResumenDiario(nombreDestinatario, resumen);
            
            var message = new EmailMessage
            {
                From = _fromEmail,
                To = new[] { destinatario },
                Subject = $"📊 Resumen Diario de Alertas - {DateTime.Now:dd/MM/yyyy}",
                HtmlBody = htmlBody
            };

            var response = await _resendClient.EmailSendAsync(message);
            return response != null && response.Content != null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al enviar email: {ex.Message}");
            return false;
        }
    }

    private string GenerarHtmlAlerta(string nombre, AlertaEmailDto alerta)
    {
        var colorNivel = alerta.Nivel == "Crítico" ? "#dc2626" : "#f97316";
        var iconoNivel = alerta.Nivel == "Crítico" ? "🚨" : "⚠️";

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f5f5f5; margin: 0; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .header {{ background: {colorNivel}; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 30px; }}
        .alerta-box {{ background: #f9fafb; border-left: 4px solid {colorNivel}; padding: 15px; margin: 20px 0; }}
        .footer {{ background: #f9fafb; padding: 20px; text-align: center; font-size: 12px; color: #666; }}
        .btn {{ display: inline-block; background: {colorNivel}; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; margin-top: 20px; }}
        .info-row {{ margin: 10px 0; padding: 10px 0; border-bottom: 1px solid #e5e7eb; }}
        .label {{ font-weight: bold; color: #374151; }}
        .value {{ color: #6b7280; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>{iconoNivel} Nueva Alerta de SLA</h1>
        </div>
        <div class='content'>
            <p>Hola <strong>{nombre}</strong>,</p>
            <p>Se ha generado una nueva alerta en el sistema Singula:</p>
            
            <div class='alerta-box'>
                <h3 style='margin-top: 0; color: {colorNivel};'>{alerta.Mensaje}</h3>
                
                <div class='info-row'>
                    <span class='label'>Tipo de Alerta:</span>
                    <span class='value'>{alerta.TipoAlerta}</span>
                </div>
                
                <div class='info-row'>
                    <span class='label'>Nivel de Prioridad:</span>
                    <span class='value' style='color: {colorNivel}; font-weight: bold;'>{alerta.Nivel}</span>
                </div>
                
                <div class='info-row'>
                    <span class='label'>Rol Afectado:</span>
                    <span class='value'>{alerta.RolAfectado}</span>
                </div>
                
                <div class='info-row'>
                    <span class='label'>Días Transcurridos:</span>
                    <span class='value'>{alerta.DiasTotal} días</span>
                </div>
                
                <div class='info-row'>
                    <span class='label'>Límite SLA:</span>
                    <span class='value'>{alerta.DiasLimite} días</span>
                </div>
                
                <div class='info-row'>
                    <span class='label'>Fecha de Solicitud:</span>
                    <span class='value'>{alerta.FechaSolicitud:dd/MM/yyyy HH:mm}</span>
                </div>
            </div>

            <p>Por favor, toma acción inmediata para resolver esta situación.</p>
            
            <a href='http://localhost:3000/notifications' class='btn'>Ver Notificaciones</a>
        </div>
        <div class='footer'>
            <p>Este es un correo automático del Sistema Singula</p>
            <p>Por favor, no respondas a este correo</p>
        </div>
    </div>
</body>
</html>";
    }

    private string GenerarHtmlResumenDiario(string nombre, ResumenAlertas resumen)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f5f5f5; margin: 0; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 30px; }}
        .stats {{ display: flex; justify-content: space-around; margin: 20px 0; }}
        .stat-box {{ text-align: center; padding: 20px; background: #f9fafb; border-radius: 8px; flex: 1; margin: 0 10px; }}
        .stat-number {{ font-size: 36px; font-weight: bold; margin: 10px 0; }}
        .stat-label {{ color: #6b7280; font-size: 14px; }}
        .critical {{ color: #dc2626; }}
        .warning {{ color: #f97316; }}
        .footer {{ background: #f9fafb; padding: 20px; text-align: center; font-size: 12px; color: #666; }}
        .btn {{ display: inline-block; background: #667eea; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; margin-top: 20px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>📊 Resumen Diario de Alertas</h1>
            <p>{DateTime.Now:dddd, dd 'de' MMMM 'de' yyyy}</p>
        </div>
        <div class='content'>
            <p>Hola <strong>{nombre}</strong>,</p>
            <p>Este es tu resumen diario de alertas del sistema Singula:</p>
            
            <div class='stats'>
                <div class='stat-box'>
                    <div class='stat-number critical'>{resumen.TotalIncumplimientos}</div>
                    <div class='stat-label'>Incumplimientos</div>
                </div>
                <div class='stat-box'>
                    <div class='stat-number warning'>{resumen.TotalPorVencer}</div>
                    <div class='stat-label'>Por Vencer (2 días)</div>
                </div>
                <div class='stat-box'>
                    <div class='stat-number'>{resumen.TotalAlertas}</div>
                    <div class='stat-label'>Total Alertas</div>
                </div>
            </div>

            <p>Recuerda revisar las alertas pendientes para tomar las acciones necesarias.</p>
            
            <a href='http://localhost:3000/notifications' class='btn'>Ver Dashboard de Notificaciones</a>
        </div>
        <div class='footer'>
            <p>Este es un correo automático del Sistema Singula</p>
            <p>Por favor, no respondas a este correo</p>
        </div>
    </div>
</body>
</html>";
    }
}

public class AlertaEmailDto
{
    public string Mensaje { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty;
    public string RolAfectado { get; set; } = string.Empty;
    public int DiasTotal { get; set; }
    public int DiasLimite { get; set; }
    public string TipoAlerta { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; }
}

public class ResumenAlertas
{
    public int TotalIncumplimientos { get; set; }
    public int TotalPorVencer { get; set; }
    public int TotalAlertas { get; set; }
}

// Wrapper class for ResendClientOptions
internal class ResendClientOptionsSnapshot : IOptionsSnapshot<ResendClientOptions>
{
    private readonly ResendClientOptions _options;

    public ResendClientOptionsSnapshot(string apiToken)
    {
        _options = new ResendClientOptions { ApiToken = apiToken };
    }

    public ResendClientOptions Value => _options;
    public ResendClientOptions Get(string? name) => _options;
}
