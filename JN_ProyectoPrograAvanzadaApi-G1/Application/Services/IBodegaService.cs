using JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Bodegas;

namespace JN_ProyectoPrograAvanzadaApi_G1.Application.Services
{
    public interface IBodegaService
    {
        Task<List<BodegaDto>> GetAllAsync(bool? activo = null);
        Task<BodegaDto?> GetByIdAsync(int bodegaId);
        Task<int> CreateAsync(CrearBodegaDto dto);
        Task<bool> UpdateAsync(int bodegaId, CrearBodegaDto dto);
        Task<bool> ToggleActivoAsync(int bodegaId);
        Task<bool> ExistsAsync(int bodegaId);
        Task<bool> DeleteAsync(int bodegaId);
    }
}

