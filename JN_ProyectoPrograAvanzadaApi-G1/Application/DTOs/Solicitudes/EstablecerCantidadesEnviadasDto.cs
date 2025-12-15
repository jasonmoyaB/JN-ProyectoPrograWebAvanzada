

namespace JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Solicitudes
{
    public class EstablecerCantidadesEnviadasDto
    {
        public List<CantidadEnviadaDetalleDto> Detalles { get; set; } = new List<CantidadEnviadaDetalleDto>();
    }

    public class CantidadEnviadaDetalleDto
    {
        public int ProductoID { get; set; }
        public decimal CantidadEnviada { get; set; }
    }
}

