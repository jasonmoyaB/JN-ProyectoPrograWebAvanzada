using JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Infrastructure.Repositories
{
    public interface IRolRepository
    {
        Task<List<Rol>> GetAllAsync();
        Task<Rol?> GetByIdAsync(int rolId);
        Task<Rol?> GetByNombreAsync(string nombreRol);
    }
}

