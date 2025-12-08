using JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Inventario;
using JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Repositories;

namespace JN_ProyectoPrograAvanzadaApi_G1.Application.Services
{
    public class InventarioService : IInventarioService
    {
        private readonly IInventarioRepository _inventarioRepository;

        public InventarioService(IInventarioRepository inventarioRepository)
        {
            _inventarioRepository = inventarioRepository;
        }

        public async Task<List<SaldoInventarioDto>> GetSaldoByBodegaAsync(int bodegaId)
        {
            var saldos = await _inventarioRepository.GetSaldoByBodegaAsync(bodegaId);
            return saldos.Select(s => new SaldoInventarioDto
            {
                BodegaID = s.BodegaID,
                BodegaNombre = s.BodegaNombre,
                ProductoID = s.ProductoID,
                ProductoNombre = s.ProductoNombre,
                ProductoSKU = s.ProductoSKU,
                Cantidad = s.Cantidad,
                StockMinimo = s.StockMinimo,
                AlertaStockBajo = s.AlertaStockBajo
            }).ToList();
        }

        public async Task<SaldoInventarioDto?> GetSaldoByBodegaAndProductoAsync(int bodegaId, int productoId)
        {
            var saldo = await _inventarioRepository.GetSaldoByBodegaAndProductoAsync(bodegaId, productoId);
            if (saldo == null) return null;

            return new SaldoInventarioDto
            {
                BodegaID = saldo.BodegaID,
                BodegaNombre = saldo.BodegaNombre,
                ProductoID = saldo.ProductoID,
                ProductoNombre = saldo.ProductoNombre,
                ProductoSKU = saldo.ProductoSKU,
                Cantidad = saldo.Cantidad,
                StockMinimo = saldo.StockMinimo,
                AlertaStockBajo = saldo.AlertaStockBajo
            };
        }

        public async Task<List<SaldoInventarioDto>> GetSaldosConAlertaStockBajoByBodegaAsync(int bodegaId)
        {
            var saldos = await _inventarioRepository.GetSaldosConAlertaStockBajoAsync();
            return saldos
                .Where(s => s.BodegaID == bodegaId)
                .Select(s => new SaldoInventarioDto
                {
                    BodegaID = s.BodegaID,
                    BodegaNombre = s.BodegaNombre,
                    ProductoID = s.ProductoID,
                    ProductoNombre = s.ProductoNombre,
                    ProductoSKU = s.ProductoSKU,
                    Cantidad = s.Cantidad,
                    StockMinimo = s.StockMinimo,
                    AlertaStockBajo = s.AlertaStockBajo
                }).ToList();
        }

        public async Task<int> GetTotalProductosByBodegaAsync(int bodegaId)
        {
            var saldos = await _inventarioRepository.GetSaldoByBodegaAsync(bodegaId);
            return saldos.Count(s => s.Cantidad > 0);
        }

        public async Task<int> GetTotalStockBajoByBodegaAsync(int bodegaId)
        {
            var saldos = await GetSaldosConAlertaStockBajoByBodegaAsync(bodegaId);
            return saldos.Count;
        }
    }
}

