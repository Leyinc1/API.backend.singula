using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Singula.Core.Core.Entities;

namespace Singula.Core.Infrastructure.Data.Configurations;

public class ArchivoExcelConfiguration : IEntityTypeConfiguration<ArchivoExcel>
{
    public void Configure(EntityTypeBuilder<ArchivoExcel> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.NombreArchivo)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.Property(x => x.RutaArchivo)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(x => x.UsuarioCarga)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(x => x.MensajeError)
            .HasMaxLength(1000);
            
        builder.HasMany(x => x.Registros)
            .WithOne(x => x.ArchivoExcel)
            .HasForeignKey(x => x.ArchivoExcelId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(x => x.FechaCarga);
        builder.HasIndex(x => x.Estado);
    }
}
