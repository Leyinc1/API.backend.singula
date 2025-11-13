using Singula.Core.Core.Entities;

namespace Singula.Core.Core.Interfaces;

public interface IRegistroKPIRepository : IRepository<RegistroKPI>
{
    Task<IEnumerable<RegistroKPI>> GetByIndicadorAsync(string indicador);
    Task<IEnumerable<RegistroKPI>> GetByAreaAsync(string area);
    Task<IEnumerable<RegistroKPI>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin);
    Task<IEnumerable<RegistroKPI>> GetByArchivoAsync(int archivoId);
    Task<RegistroKPI?> GetWithAlertasAsync(int id);
    Task<IEnumerable<string>> GetIndicadoresUnicosAsync();
    Task<IEnumerable<string>> GetAreasUnicasAsync();
    Task<decimal> GetPromedioAsync(string indicador, DateTime? desde = null, DateTime? hasta = null);
}
