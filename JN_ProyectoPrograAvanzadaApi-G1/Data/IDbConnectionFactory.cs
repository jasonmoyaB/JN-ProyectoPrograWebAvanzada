using System.Data;

namespace JN_ProyectoPrograAvanzadaApi_G1.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}



