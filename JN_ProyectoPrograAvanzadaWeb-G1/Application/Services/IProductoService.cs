using JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Productos;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Application.Services
{
    public interface IProductoService
    {
        Task<List<ProductoDto>> GetAllAsync(bool? activo=null);
        Task<ProductoDto?> GetByIdAsync(int productoId);
        Task<int> CreateAsyn(CrearProductoDto dto);
        Task<bool> UpdateAsync(int productoId, CrearProductoDto dto);
        Task<bool> ToggleActivoAsync(int productoId);
        Task<bool> ExistsAsync(int productoId);
    }
}
