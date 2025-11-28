using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Singula.Core.Core.Entities;

[Table("area")]
[Index("NombreArea", Name = "area_nombre_area_key", IsUnique = true)]
public partial class Area
{
    [Key]
    [Column("id_area")]
    public int IdArea { get; set; }

    [Column("nombre_area")]
    [StringLength(120)]
    public string NombreArea { get; set; } = null!;

    [Column("descripcion")]
    [StringLength(250)]
    public string? Descripcion { get; set; }

    [Column("activo")]
    public bool Activo { get; set; } = true;

    [InverseProperty("IdAreaNavigation")]
    public virtual ICollection<Solicitud> Solicituds { get; set; } = new List<Solicitud>();
}
