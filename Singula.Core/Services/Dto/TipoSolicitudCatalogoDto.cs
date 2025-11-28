using System.ComponentModel.DataAnnotations;

namespace Singula.Core.Services.Dto
{
    public class TipoSolicitudCatalogoDto
    {
        public int IdTipoSolicitud { get; set; }
        
        [Required(ErrorMessage = "El código es requerido")]
        [StringLength(20, ErrorMessage = "El código no puede exceder 20 caracteres")]
        public string Codigo { get; set; } = null!;
        
        [StringLength(100, ErrorMessage = "La descripción no puede exceder 100 caracteres")]
        public string? Descripcion { get; set; }
        
        public bool Activo { get; set; } = true;
    }
}
