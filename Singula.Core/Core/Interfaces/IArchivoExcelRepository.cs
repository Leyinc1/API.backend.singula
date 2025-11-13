using Singula.Core.Core.Entities;

namespace Singula.Core.Core.Interfaces;

public interface IArchivoExcelRepository : IRepository<ArchivoExcel>
{
    Task<IEnumerable<ArchivoExcel>> GetByUsuarioAsync(string usuario);
    Task<IEnumerable<ArchivoExcel>> GetByEstadoAsync(EstadoProcesamiento estado);
    Task<ArchivoExcel?> GetWithRegistrosAsync(int id);
    Task<IEnumerable<ArchivoExcel>> GetRecentesAsync(int cantidad = 10);
}
