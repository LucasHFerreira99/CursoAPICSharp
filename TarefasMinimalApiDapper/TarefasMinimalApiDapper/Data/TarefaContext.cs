using System.Data;

namespace TarefasMinimalApiDapper.Data;

public class TarefaContext
{
    public delegate Task<IDbConnection> GetConnection();
}
