using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Singula.Core.Core.Entities;

[Table("rol_registro")]
[Index("NombreRol", Name = "rol_registro_nombre_rol_key", IsUnique = true)]
public partial class RolRegistro
{
    [Key]
    [Column("id_rol_registro")]
    public int IdRolRegistro { get; set; }

    [Column("bloque_tech")]
    [StringLength(80)]
    public string? BloqueTech { get; set; }

    [Column("descripcion")]
    [StringLength(250)]
    public string? Descripcion { get; set; }

    [Column("es_activo")]
    public bool? EsActivo { get; set; }

    [Column("nombre_rol")]
    [StringLength(120)]
    public string NombreRol { get; set; } = null!;

    [InverseProperty("IdRolRegistroNavigation")]
    public virtual ICollection<Solicitud> Solicituds { get; set; } = new List<Solicitud>();
}
