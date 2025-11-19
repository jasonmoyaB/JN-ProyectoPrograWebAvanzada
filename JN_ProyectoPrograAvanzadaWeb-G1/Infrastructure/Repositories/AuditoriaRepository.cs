using Dapper;
using JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities;
using JN_ProyectoPrograAvanzadaWeb_G1.Infrastructure.Data;
using System.Data;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Infrastructure.Repositories
{
    public class AuditoriaRepository : IAuditoriaRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AuditoriaRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> RegistrarAsync(Auditoria auditoria)
        {
            using var connection = _connectionFactory.CreateConnection();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UsuarioID", auditoria.UsuarioID);
                parameters.Add("@Accion", auditoria.Accion);
                parameters.Add("@Tabla", auditoria.Tabla);
                parameters.Add("@RegistroID", auditoria.RegistroID);
                parameters.Add("@DatosAntes", auditoria.DatosAntes);
                parameters.Add("@DatosDespues", auditoria.DatosDespues);
                parameters.Add("@IPAddress", auditoria.IPAddress);
                parameters.Add("@UserAgent", auditoria.UserAgent);
                parameters.Add("@AuditoriaID", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "inv.sp_Auditoria_Registrar",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return parameters.Get<int>("@AuditoriaID");
            }
            catch
            {
                return 0;
            }
        }

        public async Task<List<Auditoria>> GetByUsuarioAsync(int usuarioId, DateTime? desde = null, DateTime? hasta = null)
        {
            using var connection = _connectionFactory.CreateConnection();

            try
            {
                var auditorias = await connection.QueryAsync<Auditoria>(
                    "inv.sp_Auditoria_GetByUsuario",
                    new
                    {
                        UsuarioID = usuarioId,
                        Desde = desde,
                        Hasta = hasta
                    },
                    commandType: CommandType.StoredProcedure);

                return auditorias.ToList();
            }
            catch
            {
                return new List<Auditoria>();
            }
        }

        public async Task<List<Auditoria>> GetByAccionAsync(string accion, DateTime? desde = null, DateTime? hasta = null)
        {
            using var connection = _connectionFactory.CreateConnection();

            try
            {
                var auditorias = await connection.QueryAsync<Auditoria>(
                    "inv.sp_Auditoria_GetByAccion",
                    new
                    {
                        Accion = accion,
                        Desde = desde,
                        Hasta = hasta
                    },
                    commandType: CommandType.StoredProcedure);

                return auditorias.ToList();
            }
            catch
            {
                return new List<Auditoria>();
            }
        }

        public async Task<List<Auditoria>> GetAllAsync(DateTime? desde = null, DateTime? hasta = null, int? limit = null)
        {
            using var connection = _connectionFactory.CreateConnection();

            try
            {
                var auditorias = await connection.QueryAsync<Auditoria>(
                    "inv.sp_Auditoria_GetAll",
                    new
                    {
                        Desde = desde,
                        Hasta = hasta,
                        Limit = limit
                    },
                    commandType: CommandType.StoredProcedure);

                return auditorias.ToList();
            }
            catch
            {
                return new List<Auditoria>();
            }
        }
    }
}

