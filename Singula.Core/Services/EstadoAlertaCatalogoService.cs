using Singula.Core.Repositories;
using Singula.Core.Services.Dto;
using Singula.Core.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public class EstadoAlertaCatalogoService : IEstadoAlertaCatalogoService
    {
        private readonly IRepository<EstadoAlertaCatalogo> _repo;

        public EstadoAlertaCatalogoService(IRepository<EstadoAlertaCatalogo> repo)
        {
            _repo = repo;
        }

        public async Task<EstadoAlertaCatalogoDto> CreateAsync(EstadoAlertaCatalogoDto dto)
        {
            var entity = new EstadoAlertaCatalogo { Codigo = dto.Codigo, Descripcion = dto.Descripcion };
            var created = await _repo.CreateAsync(entity);
            return new EstadoAlertaCatalogoDto { IdEstadoAlerta = created.IdEstadoAlerta, Codigo = created.Codigo, Descripcion = created.Descripcion };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<EstadoAlertaCatalogoDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(e => new EstadoAlertaCatalogoDto { IdEstadoAlerta = e.IdEstadoAlerta, Codigo = e.Codigo, Descripcion = e.Descripcion });
        }

        public async Task<EstadoAlertaCatalogoDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            return new EstadoAlertaCatalogoDto { IdEstadoAlerta = e.IdEstadoAlerta, Codigo = e.Codigo, Descripcion = e.Descripcion };
        }

        public async Task<EstadoAlertaCatalogoDto?> UpdateAsync(int id, EstadoAlertaCatalogoDto dto)
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
