namespace Singula.Core.Core.Interfaces;

/// <summary>
/// Unit of Work para manejar transacciones
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IArchivoExcelRepository ArchivoExcel { get; }
    IRegistroKPIRepository RegistroKPI { get; }
    IAlertaRepository Alerta { get; }
    IReporteRepository Reporte { get; }
    IPrediccionRepository Prediccion { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
