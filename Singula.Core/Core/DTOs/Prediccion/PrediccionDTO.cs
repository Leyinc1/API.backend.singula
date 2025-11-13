namespace Singula.Core.Core.DTOs.Prediccion;

public class PrediccionDTO : BaseDTO
{
    public string Indicador { get; set; } = string.Empty;
    public decimal ValorPredicho { get; set; }
    public DateTime FechaPrediccion { get; set; }
    public DateTime FechaGeneracion { get; set; }
    public decimal Confianza { get; set; }
    public string ModeloUtilizado { get; set; } = string.Empty;
}

public class CreatePrediccionDTO
{
    public string Indicador { get; set; } = string.Empty;
    public int DiasHaciaAdelante { get; set; } = 30;
}

public class PrediccionResultadoDTO
{
    public string Indicador { get; set; } = string.Empty;
    public List<PrediccionPuntoDTO> Predicciones { get; set; } = new();
    public decimal Confianza { get; set; }
    public string ModeloUtilizado { get; set; } = string.Empty;
}

public class PrediccionPuntoDTO
{
    public DateTime Fecha { get; set; }
    public decimal ValorPredicho { get; set; }
}
