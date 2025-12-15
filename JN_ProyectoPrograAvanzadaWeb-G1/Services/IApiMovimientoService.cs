namespace JN_ProyectoPrograAvanzadaWeb_G1.Services
{
    public interface IApiMovimientoService
    {
        Task<List<MovimientoDto>> GetByBodegaAsync(int bodegaId, DateTime? fechaDesde = null, DateTime? fechaHasta = null);
        Task<List<MovimientoDto>> GetByUsuarioAsync(int usuarioId, DateTime? fechaDesde = null, DateTime? fechaHasta = null);
        Task<MovimientoDto?> GetByIdAsync(int id);
        Task<int> GetCountMovimientosHoyByBodegaAsync(int bodegaId);
        Task<int> GetCountMovimientosHoyByUsuarioAsync(int usuarioId);
        Task<int> CreateTrasladoAsync(MovimientoTrasladoDto dto, int usuarioId);
    }

    public class MovimientoDto
    {
        public int MovimientoID { get; set; }
        public int BodegaID { get; set; }
        public string BodegaNombre { get; set; } = string.Empty;
        public int TipoMovimientoID { get; set; }
        public string TipoMovimientoCodigo { get; set; } = string.Empty;
        public string TipoMovimientoNombre { get; set; } = string.Empty;
        public short TipoMovimientoNaturaleza { get; set; }
        public int UsuarioID { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public DateTime FechaMovimientoUTC { get; set; }
        public string? Referencia { get; set; }
        public string? Observaciones { get; set; }
        public int? BodegaRelacionadaID { get; set; }
        public string? BodegaRelacionadaNombre { get; set; }
        public int? ProveedorID { get; set; }
        public string? ProveedorNombre { get; set; }
        public List<MovimientoDetalleDto>? Detalles { get; set; }
    }

    public class MovimientoDetalleDto
    {
        public int MovimientoDetalleID { get; set; }
        public int ProductoID { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string ProductoSKU { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
    }

    public class MovimientoTrasladoDto
    {
        public int BodegaOrigenID { get; set; }
        public int BodegaDestinoID { get; set; }
        public int TipoMovimientoID { get; set; }
        public string? Referencia { get; set; }
        public string? Observaciones { get; set; }
        public List<MovimientoTrasladoDetalleDto> Detalles { get; set; } = new List<MovimientoTrasladoDetalleDto>();
    }

    public class MovimientoTrasladoDetalleDto
    {
        public int ProductoID { get; set; }
        public int UnidadID { get; set; }
        public decimal Cantidad { get; set; }
        public decimal? CostoUnitario { get; set; }
        public int? UbicacionID { get; set; }
        public int? MotivoAjusteID { get; set; }
    }
}

