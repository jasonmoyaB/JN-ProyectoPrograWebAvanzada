using System;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Models
{
    public class Bodega
    {
        public int BodegaID { get; set; }
        public string Nombre { get; set; }
        public string? Ubicacion { get; set; }
        public bool Activo { get; set; }
        public DateTime? FechaCreacion { get; set; }

        //public int? TotalUsuarios { get; set; } 
    }
}
