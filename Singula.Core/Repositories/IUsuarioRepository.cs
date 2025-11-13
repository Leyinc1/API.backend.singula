using Singula.Core.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Repositories
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario> CreateAsync(Usuario usuario);
        Task<Usuario?> UpdateAsync(Usuario usuario);
        Task<bool> DeleteAsync(int id);
        Task<Usuario?> GetByUsernameAsync(string username);
    }
}