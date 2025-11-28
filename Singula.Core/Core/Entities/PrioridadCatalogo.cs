using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Singula.Core.Core.Entities
{
    [Table("prioridad_catalogo")]
    public class PrioridadCatalogo
    {
        [Key]
        [Column("id_prioridad")]
        public int IdPrioridad { get; set; }

        [Required]
        [Column("codigo")]
        [MaxLength(20)]
        public string Codigo { get; set; } = string.Empty;

        [Column("descripcion")]
        [MaxLength(100)]
        public string? Descripcion { get; set; }

        [Required]
        [Column("nivel")]
        public int Nivel { get; set; } // 1-4 (Baja, Media, Alta, Crítica)

        [Required]
        [Column("sla_multiplier")]
        public decimal SlaMultiplier { get; set; } // Multiplicador para cálculo de SLA

        [Column("icon")]
        [MaxLength(50)]
        public string? Icon { get; set; }

        [Column("color")]
        [MaxLength(20)]
        public string? Color { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;
    }
}
