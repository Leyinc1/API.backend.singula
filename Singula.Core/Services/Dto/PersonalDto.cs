namespace Singula.Core.Services.Dto
{
    public class PersonalDto
    {
        public int IdPersonal { get; set; }
        public int IdUsuario { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Documento { get; set; }
        public string? Estado { get; set; }
        public System.DateTime? CreadoEn { get; set; }
        public System.DateTime? ActualizadoEn { get; set; }
    }
}
