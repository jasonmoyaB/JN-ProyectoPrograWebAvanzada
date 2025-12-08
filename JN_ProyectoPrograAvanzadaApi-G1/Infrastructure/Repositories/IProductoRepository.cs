using JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities;

namespace JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Repositories
{
    public interface IProductoRepository
    {
        Task<Producto?> GetByIdAsync(int productoId);
        Task<Producto?> GetBySkuAsync(string sku);
        Task<List<Producto>> GetAllAsync(bool? activo = null);
        Task<List<Producto>> SearchAsync(string searchTerm);
        Task<int> CreateAsync(Producto producto);
        Task<bool> UpdateAsync(Producto producto);
        Task<bool> ToggleActivoAsync(int productoId);
        Task<bool> ExistsAsync(int productoId);
        Task<bool> DeleteAsync(int productoId);
    }
}

