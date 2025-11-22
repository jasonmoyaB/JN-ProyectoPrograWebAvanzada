namespace JN_ProyectoPrograAvanzadaApi_G1.Models
{
    public class MovimientoDetalleModel
    {
        public int MovimientoDetalleID { get; set; }
        public int MovimientoID { get; set; }
        public int ProductoID { get; set; }
        public int? UbicacionID { get; set; }
        public int UnidadID { get; set; }
        public decimal Cantidad { get; set; }
        public decimal? CostoUnitario { get; set; }
        public int? MotivoAjusteID { get; set; }
    }
}



