using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Singula.Core.Core.Entities;

[Table("estado_alerta_catalogo")]
[Index("Codigo", Name = "estado_alerta_catalogo_codigo_key", IsUnique = true)]
public partial class EstadoAlertaCatalogo
{
    [Key]
    [Column("id_estado_alerta")]
    public int IdEstadoAlerta { get; set; }

    [Column("codigo")]
    [StringLength(20)]
    public string Codigo { get; set; } = null!;

    [Column("descripcion")]
    [StringLength(100)]
    public string? Descripcion { get; set; }

    [InverseProperty("IdEstadoAlertaNavigation")]
    public virtual ICollection<Alertum> Alerta { get; set; } = new List<Alertum>();
}
