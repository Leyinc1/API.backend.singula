namespace Singula.Core.Core.Entities;

/// <summary>
/// Representa un reporte generado en el sistema
/// </summary>
public class Reporte : BaseEntity
{
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public TipoReporte Tipo { get; set; }
    public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;
    public string RutaArchivoPDF { get; set; } = string.Empty;
    public string UsuarioGenerador { get; set; } = string.Empty;
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? Parametros { get; set; } // JSON con parámetros del reporte
}

public enum TipoReporte
{
    General = 0,
    PorArea = 1,
    PorIndicador = 2,
    Comparativo = 3,
    Tendencias = 4
}
