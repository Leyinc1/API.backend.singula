namespace Singula.Core.Core.Entities;

/// <summary>
/// Representa una alerta generada por el sistema
/// </summary>
public class Alerta : BaseEntity
{
    public int RegistroKPIId { get; set; }
    public TipoAlerta Tipo { get; set; }
    public NivelSeveridad Nivel { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;
    public bool Leida { get; set; } = false;
    public bool EnviadaPorEmail { get; set; } = false;
    public DateTime? FechaEnvioEmail { get; set; }
    
    // Relaciones
    public RegistroKPI? RegistroKPI { get; set; }
}

public enum TipoAlerta
{
    BajoCumplimiento = 0,
    MetaAlcanzada = 1,
    TendenciaNegativa = 2,
    ValorCritico = 3
}

public enum NivelSeveridad
{
    Informativo = 0,
    Advertencia = 1,
    Critico = 2,
    Urgente = 3
}
