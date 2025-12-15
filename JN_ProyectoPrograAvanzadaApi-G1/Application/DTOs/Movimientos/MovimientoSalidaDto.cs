



using System.ComponentModel.DataAnnotations;

namespace JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Movimientos
{
    public class MovimientoSalidaDto
    {
        [Required]
        public int BodegaID { get; set; }

        [Required]
        public int TipoMovimientoID { get; set; }

        public string? Referencia { get; set; }
        public string? Observaciones { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un producto")]
        public List<MovimientoDetalleDto> Detalles { get; set; } = new List<MovimientoDetalleDto>();
    }
}

