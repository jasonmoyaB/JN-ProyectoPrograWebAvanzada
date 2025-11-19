using JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Infrastructure.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Domain.Entities.Usuario?> GetByIdAsync(int usuarioId);
        Task<Domain.Entities.Usuario?> GetByEmailAsync(string email);
        Task<Domain.Entities.Usuario?> GetByEmailAndPasswordHashAsync(string email, string passwordHash);
        Task<List<Domain.Entities.Usuario>> GetAllAsync(bool? activo = null);
        Task<List<Domain.Entities.Usuario>> GetByRolIdAsync(int rolId);
        Task<List<Domain.Entities.Usuario>> GetByBodegaIdAsync(int bodegaId);
        Task<int> CreateAsync(Domain.Entities.Usuario usuario);
        Task<bool> UpdateAsync(Domain.Entities.Usuario usuario);
        Task<bool> UpdatePasswordAsync(int usuarioId, string passwordHash);
        Task<bool> ToggleActivoAsync(int usuarioId);
        Task<bool> AssignarBodegaAsync(int usuarioId, int bodegaId);
    }
}

