namespace Singula.Core.Services.Dto
{
    public class UsuarioDto
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public int IdRolSistema { get; set; }
        public int IdEstadoUsuario { get; set; }
    }
}