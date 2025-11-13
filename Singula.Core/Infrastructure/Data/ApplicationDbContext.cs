using Microsoft.EntityFrameworkCore;
using Singula.Core.Core.Entities;

namespace Singula.Core.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    // DbSets
    public DbSet<ArchivoExcel> ArchivosExcel { get; set; }
    public DbSet<RegistroKPI> RegistrosKPI { get; set; }
    public DbSet<Alerta> Alertas { get; set; }
    public DbSet<Reporte> Reportes { get; set; }
    public DbSet<Prediccion> Predicciones { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Aplicar todas las configuraciones
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        // Configuraciones globales
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Configurar nombres de tablas en snake_case
            entityType.SetTableName(ToSnakeCase(entityType.GetTableName() ?? entityType.ClrType.Name));
            
            // Configurar nombres de propiedades en snake_case
            foreach (var property in entityType.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));
            }
        }
    }
    
    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
            
        return string.Concat(input.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString()))
            .ToLower();
    }
}
