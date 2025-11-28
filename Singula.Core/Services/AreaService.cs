using Singula.Core.Repositories;
using Singula.Core.Services.Dto;
using Singula.Core.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public class AreaService : IAreaService
    {
        private readonly IRepository<Area> _repo;

        public AreaService(IRepository<Area> repo)
        {
            _repo = repo;
        }

        public async Task<AreaDto> CreateAsync(AreaDto dto)
        {
            var entity = new Area 
            { 
                NombreArea = dto.NombreArea, 
                Descripcion = dto.Descripcion,
                Activo = dto.Activo
            };
            var created = await _repo.CreateAsync(entity);
            return new AreaDto 
            { 
                IdArea = created.IdArea, 
                NombreArea = created.NombreArea, 
                Descripcion = created.Descripcion,
                Activo = created.Activo
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;
            return await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<AreaDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(a => new AreaDto 
            { 
                IdArea = a.IdArea, 
                NombreArea = a.NombreArea, 
                Descripcion = a.Descripcion,
                Activo = a.Activo
            });
        }

        public async Task<AreaDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;
            return new AreaDto 
            { 
                IdArea = entity.IdArea, 
                NombreArea = entity.NombreArea, 
                Descripcion = entity.Descripcion,
                Activo = entity.Activo
            };
        }

        public async Task<AreaDto?> UpdateAsync(int id, AreaDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;
            entity.NombreArea = dto.NombreArea;
            entity.Descripcion = dto.Descripcion;
            entity.Activo = dto.Activo;
            await _repo.UpdateAsync(entity);
            return new AreaDto
            {
                IdArea = entity.IdArea,
                NombreArea = entity.NombreArea,
                Descripcion = entity.Descripcion,
                Activo = entity.Activo
            };
        }
    }
}
