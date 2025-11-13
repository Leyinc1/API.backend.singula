namespace Singula.Core.Services.Dto
{
    public class RolRegistroDto
    {
        public int IdRolRegistro { get; set; }
        public string? BloqueTech { get; set; }
        public string? Descripcion { get; set; }
        public bool? EsActivo { get; set; }
        public string NombreRol { get; set; } = null!;
    }
}
