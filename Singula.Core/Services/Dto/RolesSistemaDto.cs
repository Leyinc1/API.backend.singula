namespace Singula.Core.Services.Dto
{
    public class RolesSistemaDto
    {
        public int IdRolSistema { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }
        public bool? EsActivo { get; set; }
        public string? Nombre { get; set; }
    }
}
