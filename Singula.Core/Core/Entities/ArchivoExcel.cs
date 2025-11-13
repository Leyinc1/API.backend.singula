namespace Singula.Core.Core.Entities;

/// <summary>
/// Representa un archivo Excel cargado al sistema
/// </summary>
public class ArchivoExcel : BaseEntity
{
    public string NombreArchivo { get; set; } = string.Empty;
    public string RutaArchivo { get; set; } = string.Empty;
    public long TamanoBytes { get; set; }
    public DateTime FechaCarga { get; set; } = DateTime.UtcNow;
    public string UsuarioCarga { get; set; } = string.Empty;
    public EstadoProcesamiento Estado { get; set; } = EstadoProcesamiento.Pendiente;
    public string? MensajeError { get; set; }
    
    // Relaciones
    public ICollection<RegistroKPI> Registros { get; set; } = new List<RegistroKPI>();
}

public enum EstadoProcesamiento
{
    Pendiente = 0,
    Procesando = 1,
    Completado = 2,
    Error = 3
}
