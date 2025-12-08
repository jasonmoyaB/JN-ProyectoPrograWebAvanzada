namespace JN_ProyectoPrograAvanzadaWeb_G1.Services
{
    public interface IApiProductoService
    {
        Task<List<ProductoDto>> GetAllAsync(bool? activo = null);
        Task<ProductoDto?> GetByIdAsync(int id);
        Task<ProductoDto?> GetBySkuAsync(string sku);
        Task<List<ProductoDto>> SearchAsync(string term);
        Task<int> CreateAsync(CrearProductoDto dto);
        Task<bool> UpdateAsync(int id, CrearProductoDto dto);
        Task<bool> ToggleActivoAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }

    public class ProductoDto
    {
        public int ProductoID { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool EsSerializado { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class CrearProductoDto
    {
        public string SKU { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool EsSerializado { get; set; }
    }
}

