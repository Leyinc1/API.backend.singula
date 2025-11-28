using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Singula.Core.Core.Entities;

namespace Singula.Core.Infrastructure.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alertum> Alerta { get; set; }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<ConfigSla> ConfigSlas { get; set; }

    public virtual DbSet<ConfiguracionNotificacionEmail> ConfiguracionNotificacionEmails { get; set; }

    public virtual DbSet<EstadoAlertaCatalogo> EstadoAlertaCatalogos { get; set; }

    public virtual DbSet<EstadoSolicitudCatalogo> EstadoSolicitudCatalogos { get; set; }

    public virtual DbSet<EstadoUsuarioCatalogo> EstadoUsuarioCatalogos { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Personal> Personals { get; set; }

    public virtual DbSet<PrioridadCatalogo> PrioridadCatalogos { get; set; }

    public virtual DbSet<Reporte> Reportes { get; set; }

    public virtual DbSet<RolRegistro> RolRegistros { get; set; }

    public virtual DbSet<RolesSistema> RolesSistemas { get; set; }

    public virtual DbSet<Solicitud> Solicituds { get; set; }

    public virtual DbSet<TipoAlertaCatalogo> TipoAlertaCatalogos { get; set; }

    public virtual DbSet<TipoSolicitudCatalogo> TipoSolicitudCatalogos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alertum>(entity =>
        {
            entity.HasKey(e => e.IdAlerta).HasName("alerta_pkey");

            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("now()");

            entity.HasOne(d => d.IdEstadoAlertaNavigation).WithMany(p => p.Alerta)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("alerta_id_estado_alerta_fkey");

            entity.HasOne(d => d.IdSolicitudNavigation).WithMany(p => p.Alerta).HasConstraintName("alerta_id_solicitud_fkey");

            entity.HasOne(d => d.IdTipoAlertaNavigation).WithMany(p => p.Alerta)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("alerta_id_tipo_alerta_fkey");
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.IdArea).HasName("area_pkey");
        });

        modelBuilder.Entity<ConfigSla>(entity =>
        {
            entity.HasKey(e => e.IdSla).HasName("config_sla_pkey");

            entity.Property(e => e.CreadoEn).HasDefaultValueSql("now()");
            entity.Property(e => e.EsActivo).HasDefaultValue(true);

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.ConfigSlaActualizadoPorNavigations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("config_sla_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.ConfigSlaCreadoPorNavigations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("config_sla_creado_por_fkey");

            entity.HasOne(d => d.IdTipoSolicitudNavigation).WithMany(p => p.ConfigSlas)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("config_sla_id_tipo_solicitud_fkey");
        });

        modelBuilder.Entity<EstadoAlertaCatalogo>(entity =>
        {
            entity.HasKey(e => e.IdEstadoAlerta).HasName("estado_alerta_catalogo_pkey");
        });

        modelBuilder.Entity<EstadoSolicitudCatalogo>(entity =>
        {
            entity.HasKey(e => e.IdEstadoSolicitud).HasName("estado_solicitud_catalogo_pkey");
        });

        modelBuilder.Entity<EstadoUsuarioCatalogo>(entity =>
        {
            entity.HasKey(e => e.IdEstadoUsuario).HasName("estado_usuario_catalogo_pkey");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.IdPermiso).HasName("permiso_pkey");
        });

        modelBuilder.Entity<Personal>(entity =>
        {
            entity.HasKey(e => e.IdPersonal).HasName("personal_pkey");

            entity.Property(e => e.CreadoEn).HasDefaultValueSql("now()");

            entity.HasOne(d => d.IdUsuarioNavigation).WithOne(p => p.Personal).HasConstraintName("personal_id_usuario_fkey");
        });

        modelBuilder.Entity<PrioridadCatalogo>(entity =>
        {
            entity.HasKey(e => e.IdPrioridad).HasName("prioridad_catalogo_pkey");
            entity.Property(e => e.Activo).HasDefaultValue(true);
        });

        modelBuilder.Entity<Reporte>(entity =>
        {
            entity.HasKey(e => e.IdReporte).HasName("reporte_pkey");

            entity.HasIndex(e => e.FiltrosJson, "ix_reporte_filtros_gin").HasMethod("gin");

            entity.Property(e => e.FechaGeneracion).HasDefaultValueSql("now()");

            entity.HasOne(d => d.GeneradoPorNavigation).WithMany(p => p.Reportes)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("reporte_generado_por_fkey");

            entity.HasMany(d => d.IdSolicituds).WithMany(p => p.IdReportes)
                .UsingEntity<Dictionary<string, object>>(
                    "ReporteDetalle",
                    r => r.HasOne<Solicitud>().WithMany()
                        .HasForeignKey("IdSolicitud")
                        .HasConstraintName("reporte_detalle_id_solicitud_fkey"),
                    l => l.HasOne<Reporte>().WithMany()
                        .HasForeignKey("IdReporte")
                        .HasConstraintName("reporte_detalle_id_reporte_fkey"),
                    j =>
                    {
                        j.HasKey("IdReporte", "IdSolicitud").HasName("reporte_detalle_pkey");
                        j.ToTable("reporte_detalle");
                        j.HasIndex(new[] { "IdSolicitud" }, "ix_repdet_solicitud");
                        j.IndexerProperty<int>("IdReporte").HasColumnName("id_reporte");
                        j.IndexerProperty<int>("IdSolicitud").HasColumnName("id_solicitud");
                    });
        });

        modelBuilder.Entity<RolRegistro>(entity =>
        {
            entity.HasKey(e => e.IdRolRegistro).HasName("rol_registro_pkey");

            entity.Property(e => e.EsActivo).HasDefaultValue(true);
        });

        modelBuilder.Entity<RolesSistema>(entity =>
        {
            entity.HasKey(e => e.IdRolSistema).HasName("roles_sistema_pkey");

            entity.Property(e => e.EsActivo).HasDefaultValue(true);

            entity.HasMany(d => d.IdPermisos).WithMany(p => p.IdRolSistemas)
                .UsingEntity<Dictionary<string, object>>(
                    "RolPermiso",
                    r => r.HasOne<Permiso>().WithMany()
                        .HasForeignKey("IdPermiso")
                        .HasConstraintName("rol_permiso_id_permiso_fkey"),
                    l => l.HasOne<RolesSistema>().WithMany()
                        .HasForeignKey("IdRolSistema")
                        .HasConstraintName("rol_permiso_id_rol_sistema_fkey"),
                    j =>
                    {
                        j.HasKey("IdRolSistema", "IdPermiso").HasName("rol_permiso_pkey");
                        j.ToTable("rol_permiso");
                        j.HasIndex(new[] { "IdPermiso" }, "ix_rol_permiso_perm");
                        j.IndexerProperty<int>("IdRolSistema").HasColumnName("id_rol_sistema");
                        j.IndexerProperty<int>("IdPermiso").HasColumnName("id_permiso");
                    });
        });

        modelBuilder.Entity<Solicitud>(entity =>
        {
            entity.HasKey(e => e.IdSolicitud).HasName("solicitud_pkey");

            entity.Property(e => e.CreadoEn).HasDefaultValueSql("now()");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.SolicitudActualizadoPorNavigations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("solicitud_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.SolicitudCreadoPorNavigations)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("solicitud_creado_por_fkey");

            entity.HasOne(d => d.IdAreaNavigation).WithMany(p => p.Solicituds)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("solicitud_id_area_fkey");

            entity.HasOne(d => d.IdEstadoSolicitudNavigation).WithMany(p => p.Solicituds)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("solicitud_id_estado_solicitud_fkey");

            entity.HasOne(d => d.IdPersonalNavigation).WithMany(p => p.Solicituds)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("solicitud_id_personal_fkey");

            entity.HasOne(d => d.IdRolRegistroNavigation).WithMany(p => p.Solicituds)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("solicitud_id_rol_registro_fkey");

            entity.HasOne(d => d.IdSlaNavigation).WithMany(p => p.Solicituds)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("solicitud_id_sla_fkey");
        });

        modelBuilder.Entity<TipoAlertaCatalogo>(entity =>
        {
            entity.HasKey(e => e.IdTipoAlerta).HasName("tipo_alerta_catalogo_pkey");
        });

        modelBuilder.Entity<TipoSolicitudCatalogo>(entity =>
        {
            entity.HasKey(e => e.IdTipoSolicitud).HasName("tipo_solicitud_catalogo_pkey");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("usuario_pkey");

            entity.Property(e => e.CreadoEn).HasDefaultValueSql("now()");

            entity.HasOne(d => d.IdEstadoUsuarioNavigation).WithMany(p => p.Usuarios)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("usuario_id_estado_usuario_fkey");

            entity.HasOne(d => d.IdRolSistemaNavigation).WithMany(p => p.Usuarios)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("usuario_id_rol_sistema_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
