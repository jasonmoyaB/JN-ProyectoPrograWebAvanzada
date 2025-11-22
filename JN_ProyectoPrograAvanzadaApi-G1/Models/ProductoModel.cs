namespace JN_ProyectoPrograAvanzadaApi_G1.Models
{
    public class ProductoModel
    {
        public int ProductoID { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public bool EsSerializado { get; set; }
    }
}
