namespace Singula.Core.Services.Dto
{
    public class EstadoSolicitudCatalogoDto
    {
        public int IdEstadoSolicitud { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
