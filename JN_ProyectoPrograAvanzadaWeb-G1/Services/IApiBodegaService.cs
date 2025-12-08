namespace JN_ProyectoPrograAvanzadaWeb_G1.Services
{
    public interface IApiBodegaService
    {
        Task<List<BodegaDto>> GetAllAsync(bool? activo = null);
        Task<BodegaDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CrearBodegaDto dto);
        Task<bool> UpdateAsync(int id, CrearBodegaDto dto);
        Task<bool> ToggleActivoAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }

    public class BodegaDto
    {
        public int BodegaID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Ubicacion { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class CrearBodegaDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Ubicacion { get; set; }
    }
}

