using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Singula.Core.Core.Entities;

namespace Singula.Core.Infrastructure.Data.Configurations;

public class RegistroKPIConfiguration : IEntityTypeConfiguration<RegistroKPI>
{
    public void Configure(EntityTypeBuilder<RegistroKPI> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Indicador)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(x => x.ValorActual)
            .HasPrecision(18, 2);
            
        builder.Property(x => x.ValorMeta)
            .HasPrecision(18, 2);
            
        builder.Property(x => x.PorcentajeCumplimiento)
            .HasPrecision(5, 2);
            
        builder.Property(x => x.Area)
            .HasMaxLength(100);
            
        builder.Property(x => x.Responsable)
            .HasMaxLength(100);
            
        builder.Property(x => x.Observaciones)
            .HasMaxLength(500);
            
        builder.HasMany(x => x.Alertas)
            .WithOne(x => x.RegistroKPI)
            .HasForeignKey(x => x.RegistroKPIId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(x => x.Indicador);
        builder.HasIndex(x => x.FechaMedicion);
        builder.HasIndex(x => x.Area);
    }
}
