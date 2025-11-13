namespace Singula.Core.Services.Dto
{
    public class PermisoDto
    {
        public int IdPermiso { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string? Nombre { get; set; }
    }
}
