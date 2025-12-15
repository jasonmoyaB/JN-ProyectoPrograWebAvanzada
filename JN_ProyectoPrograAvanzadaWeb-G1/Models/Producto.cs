

using System;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Models
{
    public class Producto
    {
        public int ProductoID { get; set; }
        public string SKU { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool EsSerializado { get; set; }
        public bool Activo { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }
}
