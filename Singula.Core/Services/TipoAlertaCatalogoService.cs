using Singula.Core.Repositories;
using Singula.Core.Services.Dto;
using Singula.Core.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public class TipoAlertaCatalogoService : ITipoAlertaCatalogoService
    {
        private readonly IRepository<TipoAlertaCatalogo> _repo;

        public TipoAlertaCatalogoService(IRepository<TipoAlertaCatalogo> repo)
        {
            _repo = repo;
        }

        public async Task<TipoAlertaCatalogoDto> CreateAsync(TipoAlertaCatalogoDto dto)
        {
            var entity = new TipoAlertaCatalogo { Codigo = dto.Codigo, Descripcion = dto.Descripcion };
            var created = await _repo.CreateAsync(entity);
            return new TipoAlertaCatalogoDto { IdTipoAlerta = created.IdTipoAlerta, Codigo = created.Codigo, Descripcion = created.Descripcion };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<TipoAlertaCatalogoDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(t => new TipoAlertaCatalogoDto { IdTipoAlerta = t.IdTipoAlerta, Codigo = t.Codigo, Descripcion = t.Descripcion });
        }

        public async Task<TipoAlertaCatalogoDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            return new TipoAlertaCatalogoDto { IdTipoAlerta = e.IdTipoAlerta, Codigo = e.Codigo, Descripcion = e.Descripcion };
        }

        public async Task<TipoAlertaCatalogoDto?> UpdateAsync(int id, TipoAlertaCatalogoDto dto)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            e.Codigo = dto.Codigo;
            e.Descripcion = dto.Descripcion;
            await _repo.UpdateAsync(e);
            return dto;
        }
    }
}
