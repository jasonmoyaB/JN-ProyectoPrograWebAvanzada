namespace JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities
{

    // Representa el detalle de un despacho, incluyendo el producto y la cantidad despachada.
    public class DespachoDetalle
    {
        public int DespachoDetalleID { get; set; }
        public int DespachoID { get; set; }
        public int ProductoID { get; set; }
        public decimal Cantidad { get; set; }

        
        public Despacho? Despacho { get; set; }
        public Producto? Producto { get; set; }
    }
}

