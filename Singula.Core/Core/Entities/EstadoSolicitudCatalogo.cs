using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Singula.Core.Core.Entities;

[Table("estado_solicitud_catalogo")]
[Index("Codigo", Name = "estado_solicitud_catalogo_codigo_key", IsUnique = true)]
public partial class EstadoSolicitudCatalogo
{
    [Key]
    [Column("id_estado_solicitud")]
    public int IdEstadoSolicitud { get; set; }

    [Column("codigo")]
    [StringLength(20)]
    public string Codigo { get; set; } = null!;

    [Column("descripcion")]
    [StringLength(100)]
    public string? Descripcion { get; set; }

    [InverseProperty("IdEstadoSolicitudNavigation")]
    public virtual ICollection<Solicitud> Solicituds { get; set; } = new List<Solicitud>();
}
