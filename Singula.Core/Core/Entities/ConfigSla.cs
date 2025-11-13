using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Singula.Core.Core.Entities;

[Table("config_sla")]
[Index("CodigoSla", Name = "config_sla_codigo_sla_key", IsUnique = true)]
[Index("EsActivo", Name = "ix_config_sla_activo")]
[Index("IdTipoSolicitud", Name = "ix_config_sla_tipo")]
public partial class ConfigSla
{
    [Key]
    [Column("id_sla")]
    public int IdSla { get; set; }

    [Column("codigo_sla")]
    [StringLength(50)]
    public string CodigoSla { get; set; } = null!;

    [Column("descripcion")]
    [StringLength(250)]
    public string? Descripcion { get; set; }

    [Column("dias_umbral")]
    public int? DiasUmbral { get; set; }

    [Column("es_activo")]
    public bool? EsActivo { get; set; }

    [Column("id_tipo_solicitud")]
    public int IdTipoSolicitud { get; set; }

    [Column("creado_en")]
    public DateTime? CreadoEn { get; set; }

    [Column("actualizado_en")]
    public DateTime? ActualizadoEn { get; set; }

    [Column("creado_por")]
    public int? CreadoPor { get; set; }

    [Column("actualizado_por")]
    public int? ActualizadoPor { get; set; }

    [ForeignKey("ActualizadoPor")]
    [InverseProperty("ConfigSlaActualizadoPorNavigations")]
    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    [ForeignKey("CreadoPor")]
    [InverseProperty("ConfigSlaCreadoPorNavigations")]
    public virtual Usuario? CreadoPorNavigation { get; set; }

    [ForeignKey("IdTipoSolicitud")]
    [InverseProperty("ConfigSlas")]
    public virtual TipoSolicitudCatalogo IdTipoSolicitudNavigation { get; set; } = null!;

    [InverseProperty("IdSlaNavigation")]
    public virtual ICollection<Solicitud> Solicituds { get; set; } = new List<Solicitud>();
}
