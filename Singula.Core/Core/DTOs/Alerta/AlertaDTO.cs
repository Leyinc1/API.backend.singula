using Singula.Core.Core.Entities;

namespace Singula.Core.Core.DTOs.Alerta;

public class AlertaDTO : BaseDTO
{
    public int RegistroKPIId { get; set; }
    public TipoAlerta Tipo { get; set; }
    public NivelSeveridad Nivel { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public DateTime FechaGeneracion { get; set; }
    public bool Leida { get; set; }
    public bool EnviadaPorEmail { get; set; }
    public DateTime? FechaEnvioEmail { get; set; }
    
    // Datos relacionados
    public string? IndicadorKPI { get; set; }
    public decimal? ValorKPI { get; set; }
}

public class CreateAlertaDTO
{
    public int RegistroKPIId { get; set; }
    public TipoAlerta Tipo { get; set; }
    public NivelSeveridad Nivel { get; set; }
    public string Mensaje { get; set; } = string.Empty;
}

public class UpdateAlertaDTO
{
    public bool? Leida { get; set; }
    public bool? EnviadaPorEmail { get; set; }
}

public class AlertaResumenDTO
{
    public int TotalAlertas { get; set; }
    public int AlertasNoLeidas { get; set; }
    public int AlertasCriticas { get; set; }
    public List<AlertaDTO> UltimasAlertas { get; set; } = new();
}
