using System.Data.Common;

namespace Infra.PostgreSQL.Commons.Connection;

public interface IConnectionFactory
{
    DbConnection GetConnection();
}
