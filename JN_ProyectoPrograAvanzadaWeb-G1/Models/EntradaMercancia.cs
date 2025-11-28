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

        [Required]
        [StringLength(20)]
        public string TipoMovimiento { get; set; }

        [Required]
        public int ProductoID { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Cantidad { get; set; }

        [Required]
        public int BodegaID { get; set; }

        public string? Observaciones { get; set; }

        // NOTA IMPORTANTE: todas deben ser nullable
        public Usuario? Usuario { get; set; }
        public Producto? Producto { get; set; }
        public Bodega? Bodega { get; set; }
    }
}
