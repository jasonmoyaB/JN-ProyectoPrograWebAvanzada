


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Models
{
    [Table("Movimientos", Schema = "inv")]
    public class Movimiento
    {
        [Key]
        public int MovimientoID { get; set; }
        
        [Required]
        public int BodegaID { get; set; }
        
        [Required]
        public int TipoMovimientoID { get; set; }
        
        [Required]
        public int UsuarioID { get; set; }
        
        public DateTime FechaMovimientoUTC { get; set; } = DateTime.UtcNow;
        
        public string? Referencia { get; set; }
        
        public string? Observaciones { get; set; }
        
        public int? BodegaRelacionadaID { get; set; }
        
        public int? ProveedorID { get; set; }
        
        
        [NotMapped]
        public int? SolicitudID { get; set; }
        
        
        public Bodega? Bodega { get; set; }
        public TipoMovimiento? TipoMovimiento { get; set; }
        public Usuario? Usuario { get; set; }
        public ICollection<MovimientoDetalle> MovimientoDetalle { get; set; } = new List<MovimientoDetalle>();
    }
}

