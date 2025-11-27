using JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Bodegas;
using JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Productos;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Models.ViewModels
{
    public class BitacoraViewModel
    {
        public List<ProductoDto> Productos { get; set; }
        public List<BodegaDto> Bodegas { get; set; }
    }
}
