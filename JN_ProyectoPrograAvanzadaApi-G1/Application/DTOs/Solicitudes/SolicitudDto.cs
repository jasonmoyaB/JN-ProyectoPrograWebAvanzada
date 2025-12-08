namespace JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Solicitudes
{
    public class SolicitudDto
    {
        public int SolicitudID { get; set; }
        public int EstadoSolicitudID { get; set; }
        public string EstadoCodigo { get; set; } = string.Empty;
        public string EstadoNombre { get; set; } = string.Empty;
        public int BodegaID { get; set; }
        public string BodegaNombre { get; set; } = string.Empty;
        public int UsuarioID { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public DateTime FechaCreacionUTC { get; set; }
        public DateTime? FechaAprobacionUTC { get; set; }
        public DateTime? FechaEnvioUTC { get; set; }
        public DateTime? FechaEntregaUTC { get; set; }
        public string? Comentarios { get; set; }
        public int? UsuarioAprobadorID { get; set; }
        public string? UsuarioAprobadorNombre { get; set; }
        public List<SolicitudDetalleDto> Detalles { get; set; } = new List<SolicitudDetalleDto>();
    }

    public class SolicitudDetalleDto
    {
        public int SolicitudDetalleID { get; set; }
        public int ProductoID { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string ProductoSKU { get; set; } = string.Empty;
        public decimal CantidadSolicitada { get; set; }
        public decimal? CantidadEnviada { get; set; }
    }
}

