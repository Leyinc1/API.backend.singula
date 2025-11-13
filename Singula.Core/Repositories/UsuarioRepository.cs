using Microsoft.EntityFrameworkCore;
using Singula.Core.Core.Entities;
using Singula.Core.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _db;

        public UsuarioRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _db.Usuarios.AsNoTracking().ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _db.Usuarios.FindAsync(id);
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario?> UpdateAsync(Usuario usuario)
        {
            var exists = await _db.Usuarios.FindAsync(usuario.IdUsuario);
            if (exists == null) return null;
            _db.Entry(exists).CurrentValues.SetValues(usuario);
            await _db.SaveChangesAsync();
            return exists;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var exists = await _db.Usuarios.FindAsync(id);
            if (exists == null) return false;
            _db.Usuarios.Remove(exists);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<Usuario?> GetByUsernameAsync(string username)
        {
            return await _db.Usuarios.FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}