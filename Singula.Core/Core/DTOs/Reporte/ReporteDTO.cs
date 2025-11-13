using Singula.Core.Core.Entities;

namespace Singula.Core.Core.DTOs.Reporte;

public class ReporteDTO : BaseDTO
{
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public TipoReporte Tipo { get; set; }
    public DateTime FechaGeneracion { get; set; }
    public string RutaArchivoPDF { get; set; } = string.Empty;
    public string UsuarioGenerador { get; set; } = string.Empty;
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}

public class CreateReporteDTO
{
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public TipoReporte Tipo { get; set; }
    public string UsuarioGenerador { get; set; } = string.Empty;
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? Area { get; set; }
    public string? Indicador { get; set; }
}

public class ReporteGeneradoDTO
{
    public int ReporteId { get; set; }
    public string NombreArchivo { get; set; } = string.Empty;
    public string RutaDescarga { get; set; } = string.Empty;
    public byte[] ContenidoPDF { get; set; } = Array.Empty<byte>();
}
