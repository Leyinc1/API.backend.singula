namespace Singula.Core.Core.Entities;

/// <summary>
/// Representa una predicción de cumplimiento generada por el sistema
/// </summary>
public class Prediccion : BaseEntity
{
    public string Indicador { get; set; } = string.Empty;
    public decimal ValorPredicho { get; set; }
    public DateTime FechaPrediccion { get; set; }
    public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;
    public decimal Confianza { get; set; } // Porcentaje de confianza
    public string ModeloUtilizado { get; set; } = "Regresión Lineal Simple";
    public string? DatosEntrada { get; set; } // JSON con los datos usados
}
