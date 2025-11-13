using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Singula.Core.Core.Entities;

[Table("tipo_alerta_catalogo")]
[Index("Codigo", Name = "tipo_alerta_catalogo_codigo_key", IsUnique = true)]
public partial class TipoAlertaCatalogo
{
    [Key]
    [Column("id_tipo_alerta")]
    public int IdTipoAlerta { get; set; }

    [Column("codigo")]
    [StringLength(40)]
    public string Codigo { get; set; } = null!;

    [Column("descripcion")]
    [StringLength(100)]
    public string? Descripcion { get; set; }

    [InverseProperty("IdTipoAlertaNavigation")]
    public virtual ICollection<Alertum> Alerta { get; set; } = new List<Alertum>();
}
