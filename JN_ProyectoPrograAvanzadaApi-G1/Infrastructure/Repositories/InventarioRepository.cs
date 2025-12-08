using Dapper;
using JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities;
using JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Data;
using System.Data;

namespace JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Repositories
{
    public class InventarioRepository : IInventarioRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public InventarioRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<SaldoInventario>> GetSaldoByBodegaAsync(int bodegaId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var saldos = await connection.QueryAsync<SaldoInventario>(
                    "inv.sp_Inventario_GetSaldoByBodega",
                    new { BodegaID = bodegaId },
                    commandType: CommandType.StoredProcedure);

                return saldos?.ToList() ?? new List<SaldoInventario>();
            }
            catch
            {
                // Si el stored procedure no existe, retornar lista vacía
                return new List<SaldoInventario>();
            }
        }

        public async Task<SaldoInventario?> GetSaldoByBodegaAndProductoAsync(int bodegaId, int productoId)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<SaldoInventario>(
                "inv.sp_Inventario_GetSaldoByBodegaAndProducto",
                new 
                { 
                    BodegaID = bodegaId,
                    ProductoID = productoId 
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<List<SaldoInventario>> GetSaldosConAlertaStockBajoAsync()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var saldos = await connection.QueryAsync<SaldoInventario>(
                    "inv.sp_Inventario_GetSaldosConAlertaStockBajo",
                    commandType: CommandType.StoredProcedure);

                return saldos?.ToList() ?? new List<SaldoInventario>();
            }
            catch
            {
                // Si el stored procedure no existe, retornar lista vacía
                return new List<SaldoInventario>();
            }
        }

        public async Task<decimal> GetCantidadDisponibleAsync(int bodegaId, int productoId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var saldo = await connection.QueryFirstOrDefaultAsync<SaldoInventario>(
                "inv.sp_Inventario_GetSaldoByBodegaAndProducto",
                new 
                { 
                    BodegaID = bodegaId,
                    ProductoID = productoId 
                },
                commandType: CommandType.StoredProcedure);

            return saldo?.Cantidad ?? 0;
        }
    }
}

