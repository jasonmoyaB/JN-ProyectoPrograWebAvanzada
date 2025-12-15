


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Models
{
    [Table("EntradasMercancia", Schema = "inv")]
    public class EntradaMercancia
    {
        [Key]
        public int EntradaID { get; set; }

        public DateTime FechaEntrada { get; set; } = DateTime.UtcNow;

        [Required]
        public int UsuarioID { get; set; }

        [StringLength(20)]
        public string TipoMovimiento { get; set; } = string.Empty;

        [Required]
        public int TipoMovimientoID { get; set; }
        
     
        public TipoMovimiento? TipoMovimientoNav { get; set; }

        [Required]
        public int ProductoID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [Required]
        public int BodegaID { get; set; }

        public string? Observaciones { get; set; }

        
        public Usuario? Usuario { get; set; }
        public Producto? Producto { get; set; }
        public Bodega? Bodega { get; set; }
    }
}
