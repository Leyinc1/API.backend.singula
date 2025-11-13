using Singula.Core.Core.Entities;

namespace Singula.Core.Core.Interfaces;

public interface IAlertaRepository : IRepository<Alerta>
{
    Task<IEnumerable<Alerta>> GetNoLeidasAsync();
    Task<IEnumerable<Alerta>> GetByNivelAsync(NivelSeveridad nivel);
    Task<IEnumerable<Alerta>> GetByTipoAsync(TipoAlerta tipo);
    Task<IEnumerable<Alerta>> GetPendientesEnvioAsync();
    Task MarcarComoLeidaAsync(int id);
    Task MarcarComoEnviadaAsync(int id);
    Task<int> GetCountNoLeidasAsync();
}
