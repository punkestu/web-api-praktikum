using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace web_api_praktikum;

public interface DB
{
    public DbCommand CreateCommand();
    public DataRow? GetOne(SQLiteCommand command);
    public DataTable GetMany(SQLiteCommand command);
    public long? ExecuteAndId(SQLiteCommand command);
    public bool Execute(SQLiteCommand command);
}