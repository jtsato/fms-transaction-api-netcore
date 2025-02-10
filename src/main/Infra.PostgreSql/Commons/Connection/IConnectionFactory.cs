using System.Data.Common;

namespace Infra.PostgreSql.Commons.Connection;

public interface IConnectionFactory
{
    DbConnection GetConnection();
}
