using Dapper;
using JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities;
using JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Data;
using System.Data;
using DomainUsuario = JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities.Usuario;
using DomainRol = JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities.Rol;
using DomainBodega = JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities.Bodega;

namespace JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UsuarioRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<DomainUsuario?> GetByIdAsync(int usuarioId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var usuario = await connection.QueryAsync<DomainUsuario, DomainRol, DomainUsuario>(
                "inv.sp_Usuario_GetById",
                (u, r) => { u.Rol = r; return u; },
                new { UsuarioID = usuarioId },
                commandType: CommandType.StoredProcedure,
                splitOn: "RolID");

            return usuario.FirstOrDefault();
        }

        public async Task<DomainUsuario?> GetByEmailAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            string emailNormalized = email?.Trim().ToLowerInvariant() ?? string.Empty;

            var result = await connection.QueryAsync(
                "inv.sp_Usuario_GetByEmail",
                new { Email = emailNormalized },
                commandType: CommandType.StoredProcedure);

            var firstRow = result.FirstOrDefault() as dynamic;
            
            if (firstRow == null)
                return null;

            var usuario = new DomainUsuario
            {
                UsuarioID = (int)firstRow.UsuarioID,
                Nombre = (string)firstRow.Nombre,
                CorreoElectronico = (string)firstRow.CorreoElectronico,
                ContrasenaHash = (string)firstRow.ContrasenaHash,
                RolID = (int)firstRow.RolID,
                Activo = (bool)firstRow.Activo,
                FechaRegistro = (DateTime)firstRow.FechaRegistro,
                BodegaID = firstRow.BodegaID != null ? (int?)firstRow.BodegaID : null
            };

            if (firstRow.Rol_RolID != null)
            {
                usuario.Rol = new DomainRol
                {
                    RolID = (int)firstRow.Rol_RolID,
                    NombreRol = (string)firstRow.Rol_NombreRol
                };
            }

            if (firstRow.Bodega_BodegaID != null && firstRow.Bodega_Nombre != null)
            {
                usuario.Bodega = new DomainBodega
                {
                    BodegaID = (int)firstRow.Bodega_BodegaID,
                    Nombre = (string)firstRow.Bodega_Nombre,
                    Activo = firstRow.Bodega_Activo != null ? (bool)firstRow.Bodega_Activo : true
                };
            }

            return usuario;
        }

        public async Task<DomainUsuario?> GetByEmailAndPasswordHashAsync(string email, string passwordHash)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QueryAsync(
                "inv.sp_Usuario_GetByEmailAndPassword",
                new { Email = email, PasswordHash = passwordHash },
                commandType: CommandType.StoredProcedure);
            
            var firstRow = result.FirstOrDefault() as dynamic;
            
            if (firstRow == null)
                return null;

            var usuario = new DomainUsuario
            {
                UsuarioID = (int)firstRow.UsuarioID,
                Nombre = (string)firstRow.Nombre,
                CorreoElectronico = (string)firstRow.CorreoElectronico,
                ContrasenaHash = (string)firstRow.ContrasenaHash,
                RolID = (int)firstRow.RolID,
                Activo = (bool)firstRow.Activo,
                FechaRegistro = (DateTime)firstRow.FechaRegistro,
                BodegaID = firstRow.BodegaID != null ? (int?)firstRow.BodegaID : null
            };

            if (firstRow.Rol_RolID != null)
            {
                usuario.Rol = new DomainRol
                {
                    RolID = (int)firstRow.Rol_RolID,
                    NombreRol = (string)firstRow.Rol_NombreRol
                };
            }

            if (firstRow.Bodega_BodegaID != null && firstRow.Bodega_Nombre != null)
            {
                usuario.Bodega = new DomainBodega
                {
                    BodegaID = (int)firstRow.Bodega_BodegaID,
                    Nombre = (string)firstRow.Bodega_Nombre,
                    Activo = firstRow.Bodega_Activo != null ? (bool)firstRow.Bodega_Activo : true
                };
            }

            return usuario;
        }

        public async Task<List<DomainUsuario>> GetAllAsync(bool? activo = null)
        {
            using var connection = _connectionFactory.CreateConnection();

            var results = await connection.QueryAsync(
                "inv.sp_Usuario_GetAll",
                new { Activo = activo },
                commandType: CommandType.StoredProcedure);
            
            var usuarios = new List<DomainUsuario>();
            foreach (dynamic row in results)
            {
                var usuario = new DomainUsuario
                {
                    UsuarioID = (int)row.UsuarioID,
                    Nombre = (string)row.Nombre,
                    CorreoElectronico = (string)row.CorreoElectronico,
                    ContrasenaHash = (string)row.ContrasenaHash,
                    RolID = (int)row.RolID,
                    Activo = (bool)row.Activo,
                    FechaRegistro = (DateTime)row.FechaRegistro,
                    BodegaID = row.BodegaID != null ? (int?)row.BodegaID : null
                };

                if (row.Rol_RolID != null)
                {
                    usuario.Rol = new DomainRol
                    {
                        RolID = (int)row.Rol_RolID,
                        NombreRol = (string)row.Rol_NombreRol
                    };
                }

                if (row.Bodega_BodegaID != null && row.Bodega_Nombre != null)
                {
                    usuario.Bodega = new DomainBodega
                    {
                        BodegaID = (int)row.Bodega_BodegaID,
                        Nombre = (string)row.Bodega_Nombre,
                        Activo = row.Bodega_Activo != null ? (bool)row.Bodega_Activo : true
                    };
                }

                usuarios.Add(usuario);
            }

            return usuarios;
        }

        public async Task<List<DomainUsuario>> GetByRolIdAsync(int rolId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var usuarios = await connection.QueryAsync<DomainUsuario, DomainRol, DomainUsuario>(
                "inv.sp_Usuario_GetByRolId",
                (u, r) => { u.Rol = r; return u; },
                new { RolID = rolId },
                commandType: CommandType.StoredProcedure,
                splitOn: "RolID");

            return usuarios.ToList();
        }

        public async Task<List<DomainUsuario>> GetByBodegaIdAsync(int bodegaId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var usuarios = await connection.QueryAsync<DomainUsuario, DomainRol, DomainUsuario>(
                "inv.sp_Usuario_GetByBodegaId",
                (u, r) => { u.Rol = r; return u; },
                new { BodegaID = bodegaId },
                commandType: CommandType.StoredProcedure,
                splitOn: "RolID");

            return usuarios.ToList();
        }

        public async Task<int> CreateAsync(DomainUsuario usuario)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Nombre", usuario.Nombre);
            parameters.Add("@CorreoElectronico", usuario.CorreoElectronico);
            parameters.Add("@ContrasenaHash", usuario.ContrasenaHash);
            parameters.Add("@RolID", usuario.RolID);
            parameters.Add("@Activo", usuario.Activo);
            parameters.Add("@BodegaID", usuario.BodegaID);
            parameters.Add("@UsuarioID", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "inv.sp_Usuario_Create",
                parameters,
                commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("@UsuarioID");
        }

        public async Task<bool> UpdateAsync(DomainUsuario usuario)
        {
            using var connection = _connectionFactory.CreateConnection();

            var rowsAffected = await connection.ExecuteAsync(
                "inv.sp_Usuario_Update",
                new
                {
                    usuario.UsuarioID,
                    usuario.Nombre,
                    usuario.CorreoElectronico,
                    usuario.RolID,
                    usuario.Activo,
                    usuario.BodegaID
                },
                commandType: CommandType.StoredProcedure);

            return rowsAffected > 0;
        }

        public async Task<bool> UpdatePasswordAsync(int usuarioId, string passwordHash)
        {
            using var connection = _connectionFactory.CreateConnection();

            var rowsAffected = await connection.ExecuteAsync(
                "inv.sp_Usuario_UpdatePassword",
                new
                {
                    UsuarioID = usuarioId,
                    PasswordHash = passwordHash
                },
                commandType: CommandType.StoredProcedure);

            return rowsAffected > 0;
        }

        public async Task<bool> ToggleActivoAsync(int usuarioId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var rowsAffected = await connection.ExecuteAsync(
                "inv.sp_Usuario_ToggleActivo",
                new { UsuarioID = usuarioId },
                commandType: CommandType.StoredProcedure);

            return rowsAffected > 0;
        }

        public async Task<bool> AssignarBodegaAsync(int usuarioId, int bodegaId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var rowsAffected = await connection.ExecuteAsync(
                "inv.sp_Usuario_AssignarBodega",
                new
                {
                    UsuarioID = usuarioId,
                    BodegaID = bodegaId
                },
                commandType: CommandType.StoredProcedure);

            return rowsAffected > 0;
        }
    }
}

