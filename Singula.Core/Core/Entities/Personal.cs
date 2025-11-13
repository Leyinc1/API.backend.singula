using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Singula.Core.Core.Entities;

[Table("personal")]
[Index("Documento", Name = "ix_personal_documento")]
[Index("Estado", Name = "ix_personal_estado")]
[Index("IdUsuario", Name = "personal_id_usuario_key", IsUnique = true)]
public partial class Personal
{
    [Key]
    [Column("id_personal")]
    public int IdPersonal { get; set; }

    [Column("id_usuario")]
    public int IdUsuario { get; set; }

    [Column("nombres")]
    [StringLength(120)]
    public string? Nombres { get; set; }

    [Column("apellidos")]
    [StringLength(120)]
    public string? Apellidos { get; set; }

    [Column("documento")]
    [StringLength(20)]
    public string? Documento { get; set; }

    [Column("estado")]
    [StringLength(20)]
    public string? Estado { get; set; }

    [Column("creado_en")]
    public DateTime? CreadoEn { get; set; }

    [Column("actualizado_en")]
    public DateTime? ActualizadoEn { get; set; }

    [ForeignKey("IdUsuario")]
    [InverseProperty("Personal")]
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    [InverseProperty("IdPersonalNavigation")]
    public virtual ICollection<Solicitud> Solicituds { get; set; } = new List<Solicitud>();
}
