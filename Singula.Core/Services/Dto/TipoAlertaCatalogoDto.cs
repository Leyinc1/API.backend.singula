namespace Singula.Core.Services.Dto
{
    public class TipoAlertaCatalogoDto
    {
        public int IdTipoAlerta { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
