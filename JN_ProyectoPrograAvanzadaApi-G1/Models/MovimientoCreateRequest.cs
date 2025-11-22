namespace JN_ProyectoPrograAvanzadaApi_G1.Models
{
    public class MovimientoCreateRequest
    {
        public int BodegaID { get; set; }
        public int TipoMovimientoID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime FechaMovimientoUTC { get; set; } = DateTime.UtcNow;
        public string? Referencia { get; set; }
        public string? Observaciones { get; set; }
        public int? BodegaRelacionadaID { get; set; }
        public int? ProveedorID { get; set; }
        public List<MovimientoDetalleModel> Detalles { get; set; } = new();
    }
}



