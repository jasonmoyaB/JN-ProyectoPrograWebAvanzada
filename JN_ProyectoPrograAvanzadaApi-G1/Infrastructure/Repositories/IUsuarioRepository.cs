

using JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities;

namespace JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByIdAsync(int usuarioId);
        Task<Usuario?> GetByEmailAsync(string email);
        Task<Usuario?> GetByEmailAndPasswordHashAsync(string email, string passwordHash);
        Task<List<Usuario>> GetAllAsync(bool? activo = null);
        Task<List<Usuario>> GetByRolIdAsync(int rolId);
        Task<List<Usuario>> GetByBodegaIdAsync(int bodegaId);
        Task<int> CreateAsync(Usuario usuario);
        Task<bool> UpdateAsync(Usuario usuario);
        Task<bool> UpdatePasswordAsync(int usuarioId, string passwordHash);
        Task<bool> ToggleActivoAsync(int usuarioId);
        Task<bool> AssignarBodegaAsync(int usuarioId, int bodegaId);
    }
}

