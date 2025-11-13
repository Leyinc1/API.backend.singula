using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Singula.Core.Core.Entities;

namespace Singula.Core.Infrastructure.Data.Configurations;

public class AlertaConfiguration : IEntityTypeConfiguration<Alerta>
{
    public void Configure(EntityTypeBuilder<Alerta> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Mensaje)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.HasIndex(x => x.Leida);
        builder.HasIndex(x => x.Nivel);
        builder.HasIndex(x => x.FechaGeneracion);
    }
}

public class ReporteConfiguration : IEntityTypeConfiguration<Reporte>
{
    public void Configure(EntityTypeBuilder<Reporte> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Titulo)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(x => x.Descripcion)
            .HasMaxLength(500);
            
        builder.Property(x => x.RutaArchivoPDF)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(x => x.UsuarioGenerador)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(x => x.Parametros)
            .HasColumnType("jsonb");
            
        builder.HasIndex(x => x.FechaGeneracion);
        builder.HasIndex(x => x.Tipo);
    }
}

public class PrediccionConfiguration : IEntityTypeConfiguration<Prediccion>
{
    public void Configure(EntityTypeBuilder<Prediccion> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Indicador)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(x => x.ValorPredicho)
            .HasPrecision(18, 2);
            
        builder.Property(x => x.Confianza)
            .HasPrecision(5, 2);
            
        builder.Property(x => x.ModeloUtilizado)
            .HasMaxLength(100);
            
        builder.Property(x => x.DatosEntrada)
            .HasColumnType("jsonb");
            
        builder.HasIndex(x => x.Indicador);
        builder.HasIndex(x => x.FechaPrediccion);
    }
}
