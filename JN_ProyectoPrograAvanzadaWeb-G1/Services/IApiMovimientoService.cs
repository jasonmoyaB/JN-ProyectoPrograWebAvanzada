namespace JN_ProyectoPrograAvanzadaWeb_G1.Services
{
    public interface IApiMovimientoService
    {
        Task<List<MovimientoDto>> GetByBodegaAsync(int bodegaId, DateTime? fechaDesde = null, DateTime? fechaHasta = null);
        Task<List<MovimientoDto>> GetByUsuarioAsync(int usuarioId, DateTime? fechaDesde = null, DateTime? fechaHasta = null);
        Task<MovimientoDto?> GetByIdAsync(int id);
        Task<int> GetCountMovimientosHoyByBodegaAsync(int bodegaId);
        Task<int> GetCountMovimientosHoyByUsuarioAsync(int usuarioId);
    }

    public class MovimientoDto
    {
        public int MovimientoID { get; set; }
        public int BodegaID { get; set; }
        public string BodegaNombre { get; set; } = string.Empty;
        public int TipoMovimientoID { get; set; }
        public string TipoMovimientoCodigo { get; set; } = string.Empty;
        public int UsuarioID { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public DateTime FechaMovimientoUTC { get; set; }
        public string? Referencia { get; set; }
        public string? Observaciones { get; set; }
        public int? BodegaRelacionadaID { get; set; }
        public string? BodegaRelacionadaNombre { get; set; }
        public int? ProveedorID { get; set; }
        public string? ProveedorNombre { get; set; }
    }
}

