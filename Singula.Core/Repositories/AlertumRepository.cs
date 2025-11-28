using Microsoft.EntityFrameworkCore;
using Singula.Core.Core.Entities;
using Singula.Core.Infrastructure.Data;
using Singula.Core.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Singula.Core.Repositories
{
    public class AlertumRepository : IAlertumRepository
    {
        private readonly ApplicationDbContext _db;
        public AlertumRepository(ApplicationDbContext db) => _db = db;

        public async Task<Alertum> CreateAsync(Alertum entity)
        {
            _db.Alerta.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var e = await _db.Alerta.FindAsync(id);
            if (e == null) return false;
            _db.Alerta.Remove(e);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Alertum>> GetAllAsync()
        {
            return await _db.Alerta.AsNoTracking().ToListAsync();
        }

        public async Task<Alertum?> GetByIdAsync(int id)
        {
            return await _db.Alerta.FindAsync(id);
        }

        public async Task<Alertum?> UpdateAsync(Alertum entity)
        {
            _db.Alerta.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<AlertumDto>> GetByUserAsync(int userId, bool onlyUnread, int page, int pageSize)
        {
            var q = _db.Alerta
                .AsNoTracking()
                .Include(a => a.IdSolicitudNavigation)
                    .ThenInclude(s => s.IdPersonalNavigation)
                .Include(a => a.IdTipoAlertaNavigation)
                .Include(a => a.IdEstadoAlertaNavigation)
                .Where(a =>
                    a.IdSolicitudNavigation.CreadoPor == userId
                    || a.IdSolicitudNavigation.IdPersonalNavigation.IdUsuario == userId
                );

            if (onlyUnread)
                q = q.Where(a => a.FechaLectura == null);

            var list = await q
                .OrderByDescending(a => a.FechaCreacion)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return list.Select(a => new AlertumDto
            {
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

        public async Task<int> GetUnreadCountByUserAsync(int userId)
        {
            return await _db.Alerta
                .Include(a => a.IdSolicitudNavigation)
                    .ThenInclude(s => s.IdPersonalNavigation)
                .Where(a =>
                    (a.IdSolicitudNavigation.CreadoPor == userId
                     || a.IdSolicitudNavigation.IdPersonalNavigation.IdUsuario == userId)
                    && a.FechaLectura == null)
                .CountAsync();
        }

        public async Task<Alertum?> MarkAsReadAsync(int alertId, int userId)
        {
            var a = await _db.Alerta
                .Include(x => x.IdSolicitudNavigation)
                    .ThenInclude(s => s.IdPersonalNavigation)
                .FirstOrDefaultAsync(x => x.IdAlerta == alertId);

            if (a == null) return null;

            var owner = a.IdSolicitudNavigation.CreadoPor == userId
                        || a.IdSolicitudNavigation.IdPersonalNavigation.IdUsuario == userId;
            if (!owner) return null;

            a.FechaLectura = DateTime.UtcNow;
            a.ActualizadoEn = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return a;
        }
    }
}