namespace Singula.Core.Services.Dto
{
    public class TipoSolicitudCatalogoDto
    {
        public int IdTipoSolicitud { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
