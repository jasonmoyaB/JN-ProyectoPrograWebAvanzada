using Dapper;
using JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities;
using JN_ProyectoPrograAvanzadaWeb_G1.Infrastructure.Data;
using System.Data;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Infrastructure.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Producto?> GetByIdAsync(int productoId)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Producto>(
                "inv.sp_Producto_GetById",
                new { ProductoID = productoId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Producto?> GetBySkuAsync(string sku)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Producto>(
                "inv.sp_Producto_GetBySku",
                new { SKU = sku },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<List<Producto>> GetAllAsync(bool? activo = null)
        {
            using var connection = _connectionFactory.CreateConnection();

            var productos = await connection.QueryAsync<Producto>(
                "inv.sp_Producto_GetAll",
                new { Activo = activo },
                commandType: CommandType.StoredProcedure);

            return productos.ToList();
        }

        public async Task<List<Producto>> SearchAsync(string searchTerm)
        {
            using var connection = _connectionFactory.CreateConnection();

            var searchPattern = $"%{searchTerm}%";
            var productos = await connection.QueryAsync<Producto>(
                "inv.sp_Producto_Search",
                new { SearchTerm = searchPattern },
                commandType: CommandType.StoredProcedure);

            return productos.ToList();
        }

        public async Task<int> CreateAsync(Producto producto)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@SKU", producto.SKU);
            parameters.Add("@Nombre", producto.Nombre);
            parameters.Add("@Descripcion", producto.Descripcion);
            parameters.Add("@EsSerializado", producto.EsSerializado);
            parameters.Add("@Activo", producto.Activo);
            parameters.Add("@ProductoID", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "inv.sp_Producto_Create",
                parameters,
                commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("@ProductoID");
        }

        public async Task<bool> UpdateAsync(Producto producto)
        {
            using var connection = _connectionFactory.CreateConnection();

            var rowsAffected = await connection.ExecuteAsync(
                "inv.sp_Producto_Update",
                new
                {
                    producto.ProductoID,
                    producto.SKU,
                    producto.Nombre,
                    producto.Descripcion,
                    producto.EsSerializado,
                    producto.Activo
                },
                commandType: CommandType.StoredProcedure);

            return rowsAffected > 0;
        }

        public async Task<bool> ToggleActivoAsync(int productoId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var rowsAffected = await connection.ExecuteAsync(
                "inv.sp_Producto_ToggleActivo",
                new { ProductoID = productoId },
                commandType: CommandType.StoredProcedure);

            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(int productoId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var count = await connection.QuerySingleAsync<int>(
                "inv.sp_Producto_Exists",
                new { ProductoID = productoId },
                commandType: CommandType.StoredProcedure);

            return count > 0;
        }

        public async Task<bool> DeleteAsync(int productoId)
        {
            using var connection = _connectionFactory.CreateConnection();

            try
            {
                var rowsAffected = await connection.ExecuteAsync(
                    "inv.sp_Producto_Delete",
                    new { ProductoID = productoId },
                    commandType: CommandType.StoredProcedure);

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}

