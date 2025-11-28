using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Singula.Core.Core.Entities;

[Table("solicitud")]
[Index("IdArea", Name = "ix_solicitud_area")]
[Index("CreadoPor", Name = "ix_solicitud_creado_por")]
[Index("IdEstadoSolicitud", Name = "ix_solicitud_estado")]
[Index("FechaSolicitud", "FechaIngreso", Name = "ix_solicitud_fechas")]
[Index("IdPersonal", Name = "ix_solicitud_personal")]
[Index("IdRolRegistro", Name = "ix_solicitud_rol_registro")]
[Index("IdSla", Name = "ix_solicitud_sla")]
public partial class Solicitud
{
    [Key]
    [Column("id_solicitud")]
    public int IdSolicitud { get; set; }

    [Column("id_personal")]
    public int IdPersonal { get; set; }

    [Column("id_rol_registro")]
    public int IdRolRegistro { get; set; }

    [Column("id_sla")]
    public int IdSla { get; set; }

    [Column("id_area")]
    public int IdArea { get; set; }

    [Column("id_estado_solicitud")]
    public int IdEstadoSolicitud { get; set; }

    [Column("fecha_solicitud")]
    public DateTime? FechaSolicitud { get; set; }

    [Column("fecha_ingreso")]
    public DateTime? FechaIngreso { get; set; }

    [Column("num_dias_sla")]
    public int? NumDiasSla { get; set; }

    [Column("resumen_sla")]
    [StringLength(300)]
    public string? ResumenSla { get; set; }

    [Column("origen_dato")]
    [StringLength(40)]
    public string? OrigenDato { get; set; }

    [Column("prioridad")]
    [StringLength(50)]
    public string? Prioridad { get; set; }

    [Column("creado_por")]
    public int? CreadoPor { get; set; }

    [Column("creado_en")]
    public DateTime? CreadoEn { get; set; }

    [Column("actualizado_en")]
    public DateTime? ActualizadoEn { get; set; }

    [Column("actualizado_por")]
    public int? ActualizadoPor { get; set; }

    [ForeignKey("ActualizadoPor")]
    [InverseProperty("SolicitudActualizadoPorNavigations")]
    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    [InverseProperty("IdSolicitudNavigation")]
    public virtual ICollection<Alertum> Alerta { get; set; } = new List<Alertum>();

    [ForeignKey("CreadoPor")]
    [InverseProperty("SolicitudCreadoPorNavigations")]
    public virtual Usuario? CreadoPorNavigation { get; set; }

    [ForeignKey("IdArea")]
    [InverseProperty("Solicituds")]
    public virtual Area IdAreaNavigation { get; set; } = null!;

    [ForeignKey("IdEstadoSolicitud")]
    [InverseProperty("Solicituds")]
    public virtual EstadoSolicitudCatalogo IdEstadoSolicitudNavigation { get; set; } = null!;

    [ForeignKey("IdPersonal")]
    [InverseProperty("Solicituds")]
    public virtual Personal IdPersonalNavigation { get; set; } = null!;

    [ForeignKey("IdRolRegistro")]
    [InverseProperty("Solicituds")]
    public virtual RolRegistro IdRolRegistroNavigation { get; set; } = null!;

    [ForeignKey("IdSla")]
    [InverseProperty("Solicituds")]
    public virtual ConfigSla IdSlaNavigation { get; set; } = null!;

    [ForeignKey("IdSolicitud")]
    [InverseProperty("IdSolicituds")]
    public virtual ICollection<Reporte> IdReportes { get; set; } = new List<Reporte>();
}
