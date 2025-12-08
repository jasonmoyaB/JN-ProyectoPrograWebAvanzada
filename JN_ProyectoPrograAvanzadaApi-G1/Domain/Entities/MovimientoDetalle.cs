namespace JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities
{
    public class MovimientoDetalle
    {
        public int MovimientoDetalleID { get; set; }
        public int MovimientoID { get; set; }
        public int ProductoID { get; set; }
        public int? UbicacionID { get; set; }
        public int UnidadID { get; set; }
        public decimal Cantidad { get; set; }
        public decimal? CostoUnitario { get; set; }
        public int? MotivoAjusteID { get; set; }

        
        public Movimiento? Movimiento { get; set; }
        public Producto? Producto { get; set; }
        public Ubicacion? Ubicacion { get; set; }
        public UnidadMedida? UnidadMedida { get; set; }
        public MotivoAjuste? MotivoAjuste { get; set; }
        public ICollection<MovimientoDetalleSerie> Series { get; set; } = new List<MovimientoDetalleSerie>();
    }
}

