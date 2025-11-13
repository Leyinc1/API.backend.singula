namespace Singula.Core.Core.Settings;

/// <summary>
/// Configuración de la base de datos
/// </summary>
public class DatabaseSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = "TTCBD";
    public int CommandTimeout { get; set; } = 30;
    public bool EnableSensitiveDataLogging { get; set; } = false;
}

/// <summary>
/// Configuración de Email
/// </summary>
public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
}

/// <summary>
/// Configuración de almacenamiento de archivos
/// </summary>
public class FileStorageSettings
{
    public string BasePath { get; set; } = "wwwroot/uploads";
    public long MaxFileSize { get; set; } = 10485760; // 10 MB
    public List<string> AllowedExtensions { get; set; } = new() { ".xlsx", ".xls" };
}

/// <summary>
/// Configuración de alertas
/// </summary>
public class AlertaSettings
{
    public decimal UmbralCritico { get; set; } = 50; // % de cumplimiento
    public decimal UmbralAdvertencia { get; set; } = 70;
    public bool EnviarEmailAutomatico { get; set; } = true;
    public int DiasRetencionAlertas { get; set; } = 90;
}

/// <summary>
/// Configuración de predicciones
/// </summary>
public class PrediccionSettings
{
    public int MinimoDatosHistoricos { get; set; } = 5;
    public int DiasPrediccionDefecto { get; set; } = 30;
    public decimal ConfianzaMinima { get; set; } = 0.6m;
}
