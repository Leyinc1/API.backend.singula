using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Singula.Core.Core.Entities;

[Table("roles_sistema")]
[Index("Codigo", Name = "roles_sistema_codigo_key", IsUnique = true)]
public partial class RolesSistema
{
    [Key]
    [Column("id_rol_sistema")]
    public int IdRolSistema { get; set; }

    [Column("codigo")]
    [StringLength(50)]
    public string Codigo { get; set; } = null!;

    [Column("descripcion")]
    [StringLength(250)]
    public string? Descripcion { get; set; }

    [Column("es_activo")]
    public bool? EsActivo { get; set; }

    [Column("nombre")]
    [StringLength(120)]
    public string? Nombre { get; set; }

    [InverseProperty("IdRolSistemaNavigation")]
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

    [ForeignKey("IdRolSistema")]
    [InverseProperty("IdRolSistemas")]
    public virtual ICollection<Permiso> IdPermisos { get; set; } = new List<Permiso>();
}
