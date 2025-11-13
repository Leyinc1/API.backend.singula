namespace Singula.Core.Services.Dto
{
    public class UpdateUsuarioDto
    {
        public string? Username { get; set; }
        public string? Correo { get; set; }
        public string? Password { get; set; }
        public int? IdRolSistema { get; set; }
        public int? IdEstadoUsuario { get; set; }
    }
}