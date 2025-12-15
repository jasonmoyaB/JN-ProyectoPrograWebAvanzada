

using Dapper;
using JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities;
using JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Data;
using System.Data;

namespace JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Repositories
{
    public class BodegaRepository : IBodegaRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BodegaRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Bodega?> GetByIdAsync(int bodegaId)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Bodega>(
                "inv.sp_Bodega_GetById",
                new { BodegaID = bodegaId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<List<Bodega>> GetAllAsync(bool? activo = null)
        {
            using var connection = _connectionFactory.CreateConnection();

            var bodegas = await connection.QueryAsync<Bodega>(
                "inv.sp_Bodega_GetAll",
                new { Activo = activo },
                commandType: CommandType.StoredProcedure);

            return bodegas.ToList();
        }

        public async Task<int> CreateAsync(Bodega bodega)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Nombre", bodega.Nombre);
            parameters.Add("@Ubicacion", bodega.Ubicacion);
            parameters.Add("@Activo", bodega.Activo);
            parameters.Add("@BodegaID", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "inv.sp_Bodega_Create",
                parameters,
                commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("@BodegaID");
        }

        public async Task<bool> UpdateAsync(Bodega bodega)
        {
            using var connection = _connectionFactory.CreateConnection();

            var rowsAffected = await connection.ExecuteAsync(
                "inv.sp_Bodega_Update",
                new
                {
                    bodega.BodegaID,
                    bodega.Nombre,
                    bodega.Ubicacion,
                    bodega.Activo
                },
                commandType: CommandType.StoredProcedure);

            return rowsAffected > 0;
        }

        public async Task<bool> ToggleActivoAsync(int bodegaId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var rowsAffected = await connection.ExecuteAsync(
                "inv.sp_Bodega_ToggleActivo",
                new { BodegaID = bodegaId },
                commandType: CommandType.StoredProcedure);

            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(int bodegaId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var count = await connection.QuerySingleAsync<int>(
                "inv.sp_Bodega_Exists",
                new { BodegaID = bodegaId },
                commandType: CommandType.StoredProcedure);

            return count > 0;
        }

        public async Task<bool> DeleteAsync(int bodegaId)
        {
            using var connection = _connectionFactory.CreateConnection();

            try
            {
                var rowsAffected = await connection.ExecuteAsync(
                    "inv.sp_Bodega_Delete",
                    new { BodegaID = bodegaId },
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

