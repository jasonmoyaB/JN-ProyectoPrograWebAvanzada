using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Models
{
    [Table("MovimientoDetalle", Schema = "inv")]
    public class MovimientoDetalle
    {
        [Key]
        public int MovimientoDetalleID { get; set; }
        
        [Required]
        public int MovimientoID { get; set; }
        
        [Required]
        public int ProductoID { get; set; }
        
        public int? UbicacionID { get; set; }
        
        [Required]
        public int UnidadID { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Cantidad { get; set; }
        
        [Column(TypeName = "decimal(18,4)")]
        public decimal? CostoUnitario { get; set; }
        
        public int? MotivoAjusteID { get; set; }
        
       
        public Movimiento? Movimiento { get; set; }
        public Producto? Producto { get; set; }
        public UnidadMedida? UnidadMedida { get; set; }
    }
}

