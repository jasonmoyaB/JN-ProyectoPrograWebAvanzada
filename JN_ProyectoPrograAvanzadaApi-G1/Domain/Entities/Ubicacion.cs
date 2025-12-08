namespace JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities
{
    public class Ubicacion
    {
        public int UbicacionID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int? BodegaID { get; set; }
        public bool Activo { get; set; } = true;

       
        public Bodega? Bodega { get; set; }
        public ICollection<MovimientoDetalle> MovimientoDetalles { get; set; } = new List<MovimientoDetalle>();
    }
}

