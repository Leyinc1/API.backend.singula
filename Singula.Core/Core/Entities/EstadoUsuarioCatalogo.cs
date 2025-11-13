using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Singula.Core.Core.Entities;

[Table("estado_usuario_catalogo")]
[Index("Codigo", Name = "estado_usuario_catalogo_codigo_key", IsUnique = true)]
public partial class EstadoUsuarioCatalogo
{
    [Key]
    [Column("id_estado_usuario")]
    public int IdEstadoUsuario { get; set; }

    [Column("codigo")]
    [StringLength(20)]
    public string Codigo { get; set; } = null!;

    [Column("descripcion")]
    [StringLength(100)]
    public string? Descripcion { get; set; }

    [InverseProperty("IdEstadoUsuarioNavigation")]
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
