
using System.ComponentModel.DataAnnotations;

namespace JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Solicitudes
{
    public class CrearSolicitudDto
    {
        [Required]
        public int BodegaID { get; set; }

        public string? Comentarios { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un producto")]
        public List<CrearSolicitudDetalleDto> Detalles { get; set; } = new List<CrearSolicitudDetalleDto>();
    }

    public class CrearSolicitudDetalleDto
    {
        [Required]
        public int ProductoID { get; set; }

        [Required]
        [Range(0.0001, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal CantidadSolicitada { get; set; }
    }
}

