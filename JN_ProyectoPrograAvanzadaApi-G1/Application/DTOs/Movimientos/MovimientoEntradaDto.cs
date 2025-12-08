using System.ComponentModel.DataAnnotations;

namespace JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Movimientos
{
    public class MovimientoEntradaDto
    {
        [Required]
        public int BodegaID { get; set; }

        [Required]
        public int TipoMovimientoID { get; set; }

        public int? ProveedorID { get; set; }
        public string? Referencia { get; set; }
        public string? Observaciones { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un producto")]
        public List<MovimientoDetalleDto> Detalles { get; set; } = new List<MovimientoDetalleDto>();
    }

    public class MovimientoDetalleDto
    {
        [Required]
        public int ProductoID { get; set; }

        public int? UbicacionID { get; set; }

        [Required]
        public int UnidadID { get; set; }

        [Required]
        [Range(0.0001, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal Cantidad { get; set; }

        public decimal? CostoUnitario { get; set; }
        public int? MotivoAjusteID { get; set; }
        public List<string>? NumerosSerie { get; set; } 
    }
}

