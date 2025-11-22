namespace JN_ProyectoPrograAvanzadaApi_G1.Models
{
    public class MovimientoModel
    {
        public int MovimientoID { get; set; }
        public int BodegaID { get; set; }
        public int TipoMovimientoID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime FechaMovimientoUTC { get; set; }
        public string? Referencia { get; set; }
        public string? Observaciones { get; set; }
        public int? BodegaRelacionadaID { get; set; }
        public int? ProveedorID { get; set; }

        public string? BodegaNombre { get; set; }
        public string? TipoMovimientoCodigo { get; set; }
        public string? UsuarioNombre { get; set; }
    }
}



