using Microsoft.EntityFrameworkCore.Storage;
using Singula.Core.Core.Interfaces;
using Singula.Core.Infrastructure.Data;

namespace Singula.Core.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    
    public IArchivoExcelRepository ArchivoExcel { get; }
    public IRegistroKPIRepository RegistroKPI { get; }
    public IAlertaRepository Alerta { get; }
    public IReporteRepository Reporte { get; }
    public IPrediccionRepository Prediccion { get; }
    
    public UnitOfWork(
        ApplicationDbContext context,
        IArchivoExcelRepository archivoExcelRepository,
        IRegistroKPIRepository registroKPIRepository,
        IAlertaRepository alertaRepository,
        IReporteRepository reporteRepository,
        IPrediccionRepository prediccionRepository)
    {
        _context = context;
        ArchivoExcel = archivoExcelRepository;
        RegistroKPI = registroKPIRepository;
        Alerta = alertaRepository;
        Reporte = reporteRepository;
        Prediccion = prediccionRepository;
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }
    
    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            await _transaction?.CommitAsync()!;
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }
    
    public async Task RollbackTransactionAsync()
    {
        await _transaction?.RollbackAsync()!;
        _transaction?.Dispose();
        _transaction = null;
    }
    
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
