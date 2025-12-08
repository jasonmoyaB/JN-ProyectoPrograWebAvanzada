using JN_ProyectoPrograAvanzadaWeb_G1.Services;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Models.ViewModels
{
    public class BitacoraViewModel
    {
        public List<ProductoDto> Productos { get; set; } = new List<ProductoDto>();
        public List<BodegaDto> Bodegas { get; set; } = new List<BodegaDto>();
    }
}
