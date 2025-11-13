using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Singula.Core.Core.Entities;

[Table("tipo_solicitud_catalogo")]
[Index("Codigo", Name = "tipo_solicitud_catalogo_codigo_key", IsUnique = true)]
public partial class TipoSolicitudCatalogo
{
    [Key]
    [Column("id_tipo_solicitud")]
    public int IdTipoSolicitud { get; set; }

    [Column("codigo")]
    [StringLength(20)]
    public string Codigo { get; set; } = null!;

    [Column("descripcion")]
    [StringLength(100)]
    public string? Descripcion { get; set; }

    [InverseProperty("IdTipoSolicitudNavigation")]
    public virtual ICollection<ConfigSla> ConfigSlas { get; set; } = new List<ConfigSla>();
}
