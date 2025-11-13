using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Singula.Core.Core.Entities;

[Table("alerta")]
[Index("IdEstadoAlerta", Name = "ix_alerta_estado")]
[Index("IdSolicitud", Name = "ix_alerta_solicitud")]
[Index("IdTipoAlerta", Name = "ix_alerta_tipo")]
public partial class Alertum
{
    [Key]
    [Column("id_alerta")]
    public int IdAlerta { get; set; }

    [Column("id_solicitud")]
    public int IdSolicitud { get; set; }

    [Column("id_tipo_alerta")]
    public int IdTipoAlerta { get; set; }

    [Column("id_estado_alerta")]
    public int IdEstadoAlerta { get; set; }

    [Column("nivel")]
    [StringLength(20)]
    public string? Nivel { get; set; }

    [Column("mensaje")]
    public string? Mensaje { get; set; }

    [Column("enviado_email")]
    public bool? EnviadoEmail { get; set; }

    [Column("fecha_creacion")]
    public DateTime? FechaCreacion { get; set; }

    [Column("fecha_lectura")]
    public DateTime? FechaLectura { get; set; }

    [Column("actualizado_en")]
    public DateTime? ActualizadoEn { get; set; }

    [ForeignKey("IdEstadoAlerta")]
    [InverseProperty("Alerta")]
    public virtual EstadoAlertaCatalogo IdEstadoAlertaNavigation { get; set; } = null!;

    [ForeignKey("IdSolicitud")]
    [InverseProperty("Alerta")]
    public virtual Solicitud IdSolicitudNavigation { get; set; } = null!;

    [ForeignKey("IdTipoAlerta")]
    [InverseProperty("Alerta")]
    public virtual TipoAlertaCatalogo IdTipoAlertaNavigation { get; set; } = null!;
}
