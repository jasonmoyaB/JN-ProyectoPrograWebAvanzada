using JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Productos;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Application.Services
{
    public interface IProductoService
    {
        Task<List<ProductoDto>> GetAllAsync(bool? activo = null);
        Task<ProductoDto?> GetByIdAsync(int productoId);
        Task<ProductoDto?> GetBySkuAsync(string sku);
        Task<List<ProductoDto>> SearchAsync(string searchTerm);
        Task<int> CreateAsync(CrearProductoDto dto);
        Task<bool> UpdateAsync(int productoId, CrearProductoDto dto);
        Task<bool> ToggleActivoAsync(int productoId);
        Task<bool> ExistsAsync(int productoId);
        Task<bool> DeleteAsync(int productoId);
    }
}


