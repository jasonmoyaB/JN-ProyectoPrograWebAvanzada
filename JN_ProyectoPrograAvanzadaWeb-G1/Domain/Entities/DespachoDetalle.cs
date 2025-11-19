namespace JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities
{
    public class DespachoDetalle
    {
        public int DespachoDetalleID { get; set; }
        public int DespachoID { get; set; }
        public int ProductoID { get; set; }
        public decimal Cantidad { get; set; }

        // Navegación
        public Despacho? Despacho { get; set; }
        public Producto? Producto { get; set; }
    }
}

