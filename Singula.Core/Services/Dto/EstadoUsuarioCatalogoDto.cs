namespace Singula.Core.Services.Dto
{
    public class EstadoUsuarioCatalogoDto
    {
        public int IdEstadoUsuario { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
