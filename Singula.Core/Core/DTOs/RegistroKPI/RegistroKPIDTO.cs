namespace Singula.Core.Core.DTOs.RegistroKPI;

public class RegistroKPIDTO : BaseDTO
{
    public int ArchivoExcelId { get; set; }
    public string Indicador { get; set; } = string.Empty;
    public decimal ValorActual { get; set; }
    public decimal ValorMeta { get; set; }
    public decimal PorcentajeCumplimiento { get; set; }
    public DateTime FechaMedicion { get; set; }
    public string? Area { get; set; }
    public string? Responsable { get; set; }
    public string? Observaciones { get; set; }
}

public class CreateRegistroKPIDTO
{
    public int ArchivoExcelId { get; set; }
    public string Indicador { get; set; } = string.Empty;
    public decimal ValorActual { get; set; }
    public decimal ValorMeta { get; set; }
    public DateTime FechaMedicion { get; set; }
    public string? Area { get; set; }
    public string? Responsable { get; set; }
    public string? Observaciones { get; set; }
}

public class UpdateRegistroKPIDTO
{
    public string? Indicador { get; set; }
    public decimal? ValorActual { get; set; }
    public decimal? ValorMeta { get; set; }
    public DateTime? FechaMedicion { get; set; }
    public string? Area { get; set; }
    public string? Responsable { get; set; }
    public string? Observaciones { get; set; }
}

public class KPIDashboardDTO
{
    public string Indicador { get; set; } = string.Empty;
    public decimal UltimoValor { get; set; }
    public decimal Meta { get; set; }
    public decimal PorcentajeCumplimiento { get; set; }
    public string Tendencia { get; set; } = string.Empty; // "Ascendente", "Descendente", "Estable"
    public List<KPIHistoricoDTO> Historico { get; set; } = new();
}

public class KPIHistoricoDTO
{
    public DateTime Fecha { get; set; }
    public decimal Valor { get; set; }
}
