using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Models
{
    [Table("TiposMovimiento", Schema = "inv")]
    public class TipoMovimiento
    {
        [Key]
        public int TipoMovimientoID { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Codigo { get; set; } = string.Empty;
        
        [Required]
        public short Naturaleza { get; set; } 

        [NotMapped]
        public string Nombre { get; set; } = string.Empty;
        
        [NotMapped]
        public bool Activo { get; set; } = true;
    }
}

