namespace Singula.Core.Services.Dto
{
    public class EstadoAlertaCatalogoDto
    {
        public int IdEstadoAlerta { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
