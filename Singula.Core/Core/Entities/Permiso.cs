using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Singula.Core.Core.Entities;

[Table("permiso")]
[Index("Codigo", Name = "permiso_codigo_key", IsUnique = true)]
public partial class Permiso
{
    [Key]
    [Column("id_permiso")]
    public int IdPermiso { get; set; }

    [Column("codigo")]
    [StringLength(50)]
    public string Codigo { get; set; } = null!;

    [Column("descripcion")]
    [StringLength(250)]
    public string? Descripcion { get; set; }

    [Column("nombre")]
    [StringLength(120)]
    public string? Nombre { get; set; }

    [ForeignKey("IdPermiso")]
    [InverseProperty("IdPermisos")]
    public virtual ICollection<RolesSistema> IdRolSistemas { get; set; } = new List<RolesSistema>();
}
