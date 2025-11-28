using Singula.Core.Repositories;
using Singula.Core.Services.Dto;
using Singula.Core.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public class AlertumService : IAlertumService
    {
        private readonly IRepository<Alertum> _repo; // generic for basic ops
        private readonly IAlertumRepository _alertRepo; // specialized queries

        public AlertumService(IRepository<Alertum> repo, IAlertumRepository alertRepo)
        {
            _repo = repo;
            _alertRepo = alertRepo;
        }

        public async Task<AlertumDto> CreateAsync(AlertumDto dto)
        {
            var entity = new Alertum {
                IdSolicitud = dto.IdSolicitud,
                IdTipoAlerta = dto.IdTipoAlerta,
                IdEstadoAlerta = dto.IdEstadoAlerta,
                Nivel = dto.Nivel,
                Mensaje = dto.Mensaje,
                EnviadoEmail = dto.EnviadoEmail
            };
            var created = await _repo.CreateAsync(entity);
            return new AlertumDto {
                IdAlerta = created.IdAlerta,
                IdSolicitud = created.IdSolicitud,
                IdTipoAlerta = created.IdTipoAlerta,
                IdEstadoAlerta = created.IdEstadoAlerta,
                Nivel = created.Nivel,
                Mensaje = created.Mensaje,
                EnviadoEmail = created.EnviadoEmail,
                FechaCreacion = created.FechaCreacion,
                FechaLectura = created.FechaLectura,
                ActualizadoEn = created.ActualizadoEn
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<AlertumDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(a => new AlertumDto {
                IdAlerta = a.IdAlerta,
                IdSolicitud = a.IdSolicitud,
                IdTipoAlerta = a.IdTipoAlerta,
                IdEstadoAlerta = a.IdEstadoAlerta,
                Nivel = a.Nivel,
                Mensaje = a.Mensaje,
                EnviadoEmail = a.EnviadoEmail,
                FechaCreacion = a.FechaCreacion,
                FechaLectura = a.FechaLectura,
                ActualizadoEn = a.ActualizadoEn
            });
        }

        public async Task<AlertumDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            return new AlertumDto {
                IdAlerta = e.IdAlerta,
                IdSolicitud = e.IdSolicitud,
                IdTipoAlerta = e.IdTipoAlerta,
                IdEstadoAlerta = e.IdEstadoAlerta,
                Nivel = e.Nivel,
                Mensaje = e.Mensaje,
                EnviadoEmail = e.EnviadoEmail,
                FechaCreacion = e.FechaCreacion,
                FechaLectura = e.FechaLectura,
                ActualizadoEn = e.ActualizadoEn
            };
        }

        public async Task<AlertumDto?> UpdateAsync(int id, AlertumDto dto)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            e.IdSolicitud = dto.IdSolicitud;
            e.IdTipoAlerta = dto.IdTipoAlerta;
            e.IdEstadoAlerta = dto.IdEstadoAlerta;
            e.Nivel = dto.Nivel;
            e.Mensaje = dto.Mensaje;
            e.EnviadoEmail = dto.EnviadoEmail;
            e.ActualizadoEn = dto.ActualizadoEn;
            await _repo.UpdateAsync(e);
            return dto;
        }

        // New delegating methods
        public async Task<IEnumerable<AlertumDto>> GetByUserAsync(int userId, bool onlyUnread = false, int page = 1, int pageSize = 20)
            => await _alertRepo.GetByUserAsync(userId, onlyUnread, page, pageSize);

        public async Task<int> GetUnreadCountByUserAsync(int userId)
            => await _alertRepo.GetUnreadCountByUserAsync(userId);

        public async Task<bool> MarkAsReadAsync(int alertId, int userId)
        {
            var updated = await _alertRepo.MarkAsReadAsync(alertId, userId);
            return updated != null;
        }
    }
}
