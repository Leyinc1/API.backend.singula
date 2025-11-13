using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Singula.Core.Core.Entities;

[Table("usuario")]
[Index("IdEstadoUsuario", Name = "ix_usuario_estado")]
[Index("IdRolSistema", Name = "ix_usuario_rol")]
[Index("Correo", Name = "usuario_correo_key", IsUnique = true)]
[Index("Username", Name = "usuario_username_key", IsUnique = true)]
public partial class Usuario
{
    [Key]
    [Column("id_usuario")]
    public int IdUsuario { get; set; }

    [Column("username")]
    [StringLength(100)]
    public string Username { get; set; } = null!;

    [Column("correo")]
    [StringLength(190)]
    public string Correo { get; set; } = null!;

    [Column("password_hash")]
    [StringLength(255)]
    public string? PasswordHash { get; set; }

    [Column("id_rol_sistema")]
    public int IdRolSistema { get; set; }

    [Column("id_estado_usuario")]
    public int IdEstadoUsuario { get; set; }

    [Column("creado_en")]
    public DateTime? CreadoEn { get; set; }

    [Column("actualizado_en")]
    public DateTime? ActualizadoEn { get; set; }

    [Column("ultimo_login")]
    public DateTime? UltimoLogin { get; set; }

    [InverseProperty("ActualizadoPorNavigation")]
    public virtual ICollection<ConfigSla> ConfigSlaActualizadoPorNavigations { get; set; } = new List<ConfigSla>();

    [InverseProperty("CreadoPorNavigation")]
    public virtual ICollection<ConfigSla> ConfigSlaCreadoPorNavigations { get; set; } = new List<ConfigSla>();

    [ForeignKey("IdEstadoUsuario")]
    [InverseProperty("Usuarios")]
    public virtual EstadoUsuarioCatalogo IdEstadoUsuarioNavigation { get; set; } = null!;

    [ForeignKey("IdRolSistema")]
    [InverseProperty("Usuarios")]
    public virtual RolesSistema IdRolSistemaNavigation { get; set; } = null!;

    [InverseProperty("IdUsuarioNavigation")]
    public virtual Personal? Personal { get; set; }

    [InverseProperty("GeneradoPorNavigation")]
    public virtual ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();

    [InverseProperty("ActualizadoPorNavigation")]
    public virtual ICollection<Solicitud> SolicitudActualizadoPorNavigations { get; set; } = new List<Solicitud>();

    [InverseProperty("CreadoPorNavigation")]
    public virtual ICollection<Solicitud> SolicitudCreadoPorNavigations { get; set; } = new List<Solicitud>();
}
