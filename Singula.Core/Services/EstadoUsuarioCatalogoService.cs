using Singula.Core.Repositories;
using Singula.Core.Services.Dto;
using Singula.Core.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public class EstadoUsuarioCatalogoService : IEstadoUsuarioCatalogoService
    {
        private readonly IRepository<EstadoUsuarioCatalogo> _repo;

        public EstadoUsuarioCatalogoService(IRepository<EstadoUsuarioCatalogo> repo)
        {
            _repo = repo;
        }

        public async Task<EstadoUsuarioCatalogoDto> CreateAsync(EstadoUsuarioCatalogoDto dto)
        {
            var entity = new EstadoUsuarioCatalogo { Codigo = dto.Codigo, Descripcion = dto.Descripcion };
            var created = await _repo.CreateAsync(entity);
            return new EstadoUsuarioCatalogoDto { IdEstadoUsuario = created.IdEstadoUsuario, Codigo = created.Codigo, Descripcion = created.Descripcion };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<EstadoUsuarioCatalogoDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(e => new EstadoUsuarioCatalogoDto { IdEstadoUsuario = e.IdEstadoUsuario, Codigo = e.Codigo, Descripcion = e.Descripcion });
        }

        public async Task<EstadoUsuarioCatalogoDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            return new EstadoUsuarioCatalogoDto { IdEstadoUsuario = e.IdEstadoUsuario, Codigo = e.Codigo, Descripcion = e.Descripcion };
        }

        public async Task<EstadoUsuarioCatalogoDto?> UpdateAsync(int id, EstadoUsuarioCatalogoDto dto)
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
