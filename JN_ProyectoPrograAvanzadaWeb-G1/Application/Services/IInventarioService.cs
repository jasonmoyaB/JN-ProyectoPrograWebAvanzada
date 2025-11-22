using JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Inventario;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Application.Services
{
    public interface IInventarioService
    {
        Task<List<SaldoInventarioDto>> GetSaldoByBodegaAsync(int bodegaId);
        Task<SaldoInventarioDto?> GetSaldoByBodegaAndProductoAsync(int bodegaId, int productoId);
        Task<List<SaldoInventarioDto>> GetSaldosConAlertaStockBajoByBodegaAsync(int bodegaId);
        Task<int> GetTotalProductosByBodegaAsync(int bodegaId);
        Task<int> GetTotalStockBajoByBodegaAsync(int bodegaId);
    }
}

