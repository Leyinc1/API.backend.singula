using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Singula.Core.Core.Entities;

[Table("configuracion_notificaciones_email")]
public class ConfiguracionNotificacionEmail
{
    [Key]
    [Column("id_configuracion")]
    public int IdConfiguracion { get; set; }

    [Column("id_usuario")]
    public int IdUsuario { get; set; }

    [Column("email")]
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Column("notificar_incumplimientos")]
    public bool NotificarIncumplimientos { get; set; } = true;

    [Column("notificar_por_vencer")]
    public bool NotificarPorVencer { get; set; } = true;

    [Column("enviar_resumen_diario")]
    public bool EnviarResumenDiario { get; set; } = false;

    [Column("hora_resumen_diario")]
    public TimeSpan HoraResumenDiario { get; set; } = new TimeSpan(8, 0, 0); // 8:00 AM

    [Column("activo")]
    public bool Activo { get; set; } = true;

    [Column("fecha_creacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    [Column("fecha_actualizacion")]
    public DateTime? FechaActualizacion { get; set; }

    // Navegación
    [ForeignKey("IdUsuario")]
    public virtual Usuario? Usuario { get; set; }
}
