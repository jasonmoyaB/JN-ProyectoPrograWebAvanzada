using JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Infrastructure.Repositories
{
    public interface IInventarioRepository
    {
        Task<List<SaldoInventario>> GetSaldoByBodegaAsync(int bodegaId);
        Task<SaldoInventario?> GetSaldoByBodegaAndProductoAsync(int bodegaId, int productoId);
        Task<List<SaldoInventario>> GetSaldosConAlertaStockBajoAsync();
        Task<decimal> GetCantidadDisponibleAsync(int bodegaId, int productoId);
    }
}

