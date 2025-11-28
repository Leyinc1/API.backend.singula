using System.ComponentModel.DataAnnotations;

namespace Singula.Core.Services.Dto
{
    public class AreaDto
    {
        public int IdArea { get; set; }
        
        [Required(ErrorMessage = "El nombre del área es requerido")]
        [StringLength(120, ErrorMessage = "El nombre no puede exceder 120 caracteres")]
        public string NombreArea { get; set; } = null!;
        
        [StringLength(250, ErrorMessage = "La descripción no puede exceder 250 caracteres")]
        public string? Descripcion { get; set; }
        
        public bool Activo { get; set; } = true;
    }
}
