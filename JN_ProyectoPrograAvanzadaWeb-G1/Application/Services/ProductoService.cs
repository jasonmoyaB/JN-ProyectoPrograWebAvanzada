using JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Productos;
using JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities;
using JN_ProyectoPrograAvanzadaWeb_G1.Infrastructure.Repositories;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Application.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }


        public async Task<List<ProductoDto>> GetAllAsync(bool? activo = null)
        {
           var productos = await _productoRepository.GetAllAsync(activo);
            return productos.Select(p => new ProductoDto
            {
                ProductoID = p.ProductoID,
                SKU = p.SKU,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                EsSerializado = p.EsSerializado,
                Activo = p.Activo,
                FechaCreacion = p.FechaCreacion
            }).ToList();
        }

        public async Task<ProductoDto?> GetByIdAsync(int productoId)
        {
            var producto = await _productoRepository.GetByIdAsync(productoId);
            if (producto == null) return null;
            return new ProductoDto
            {
                ProductoID = producto.ProductoID,
                SKU = producto.SKU,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                EsSerializado = producto.EsSerializado,
               
            };
        }
        public async Task<int> CreateAsyn(CrearProductoDto dto)
        {
            var producto = new Producto
            {
                SKU = dto.SKU,
                Nombre = dto.Nombre,
                Activo = true
            };
            return await _productoRepository.CreateAsync(producto);
        }
        public async Task<bool> UpdateAsync(int productoId, CrearProductoDto dto)
        {
            var producto = await _productoRepository.GetByIdAsync(productoId);
            if (producto == null) return false;
            producto.SKU = dto.SKU;
            producto.Nombre = dto.Nombre;
            return await _productoRepository.UpdateAsync(producto);
        }

        public async Task<bool> ToggleActivoAsync(int productoId)
        {
            return await _productoRepository.ToggleActivoAsync(productoId);
        }
        public async Task<bool> ExistsAsync(int productoId)
        {
            return await _productoRepository.ExistsAsync(productoId);
        }
    }
}
