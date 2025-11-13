namespace Singula.Core.Services.Dto
{
    public class CreateUsuarioDto
    {
        public string Username { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int IdRolSistema { get; set; }
        public int IdEstadoUsuario { get; set; }
    }
}