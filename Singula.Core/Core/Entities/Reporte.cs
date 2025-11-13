using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Singula.Core.Core.Entities;

[Table("reporte")]
[Index("GeneradoPor", "FechaGeneracion", Name = "ix_reporte_generado", IsDescending = new[] { false, true })]
[Index("TipoReporte", "Formato", Name = "ix_reporte_tipo_formato")]
public partial class Reporte
{
    [Key]
    [Column("id_reporte")]
    public int IdReporte { get; set; }

    [Column("tipo_reporte")]
    [StringLength(40)]
    public string? TipoReporte { get; set; }

    [Column("formato")]
    [StringLength(10)]
    public string? Formato { get; set; }

    [Column("filtros_json", TypeName = "jsonb")]
    public string? FiltrosJson { get; set; }

    [Column("generado_por")]
    public int GeneradoPor { get; set; }

    [Column("fecha_generacion")]
    public DateTime? FechaGeneracion { get; set; }

    [Column("ruta_archivo")]
    [StringLength(400)]
    public string? RutaArchivo { get; set; }

    [ForeignKey("GeneradoPor")]
    [InverseProperty("Reportes")]
    public virtual Usuario GeneradoPorNavigation { get; set; } = null!;

    [ForeignKey("IdReporte")]
    [InverseProperty("IdReportes")]
    public virtual ICollection<Solicitud> IdSolicituds { get; set; } = new List<Solicitud>();
}
