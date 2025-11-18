using JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Infrastructure.Repositories
{
    public interface IBodegaRepository
    {
        Task<Bodega?> GetByIdAsync(int bodegaId);
        Task<List<Bodega>> GetAllAsync(bool? activo = null);
        Task<int> CreateAsync(Bodega bodega);
        Task<bool> UpdateAsync(Bodega bodega);
        Task<bool> ToggleActivoAsync(int bodegaId);
        Task<bool> ExistsAsync(int bodegaId);
    }
}

