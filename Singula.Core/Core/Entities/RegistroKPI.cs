namespace Singula.Core.Core.Entities;

/// <summary>
/// Representa un registro individual de KPI extraído del Excel
/// </summary>
public class RegistroKPI : BaseEntity
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
    
    // Relaciones
    public ArchivoExcel? ArchivoExcel { get; set; }
    public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
}
