using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace web_api_praktikum;

public class SqliteImpl : DB
{
      private Mutex mutex = new Mutex();
      private SQLiteConnection sqlite_conn;
      public SqliteImpl(string connstr)
      {
            sqlite_conn = CreateConnection(connstr);
            CreateTable();
      }
      private SQLiteConnection CreateConnection(string connstr)
      {
            sqlite_conn = new SQLiteConnection(connstr);
            return sqlite_conn;
      }
      private void CreateTable()
      {
            sqlite_conn.Open();
            SQLiteCommand sqlite_cmd;
            string CheckTableUsers = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'users';";
            SQLiteDataReader sqlite_datareader;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = CheckTableUsers;

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            if (!sqlite_datareader.Read())
            {
                  sqlite_cmd.Dispose();
                  string CreateTableUsers = "CREATE TABLE users (id INTEGER PRIMARY KEY AUTOINCREMENT, username TEXT UNIQUE NOT NULL, password TEXT NOT NULL);";
                  sqlite_cmd = sqlite_conn.CreateCommand();
                  sqlite_cmd.CommandText = CreateTableUsers;
                  sqlite_cmd.ExecuteNonQuery();
            }
            sqlite_datareader.Close();
            sqlite_cmd.Dispose();
            string CheckTableProducts = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'products';";
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = CheckTableProducts;

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            if (!sqlite_datareader.Read())
            {
                  sqlite_cmd.Dispose();
                  string CreateTableProducts = "CREATE TABLE products (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT UNIQUE NOT NULL, price INTEGER NOT NULL, stock INTEGER NOT NULL);";
                  sqlite_cmd = sqlite_conn.CreateCommand();
                  sqlite_cmd.CommandText = CreateTableProducts;
                  sqlite_cmd.ExecuteNonQuery();
            }
            sqlite_datareader.Close();
            sqlite_cmd.Dispose();
            sqlite_conn.Close();
      }
      public DbCommand CreateCommand()
      {
            mutex.WaitOne();
            sqlite_conn.Open();
            return sqlite_conn.CreateCommand();
      }
      public DataRow? GetOne(SQLiteCommand command)
      {
            SQLiteDataAdapter da = new SQLiteDataAdapter(command);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count == 0)
            {
                  command.Dispose();
                  sqlite_conn.Close();
                  mutex.ReleaseMutex();
                  return null;
            }
            DataRow dr = dt.Rows[0];

            command.Dispose();
            sqlite_conn.Close();
            mutex.ReleaseMutex();
            return dr;
      }
      public DataTable GetMany(SQLiteCommand command)
      {
            SQLiteDataAdapter da = new SQLiteDataAdapter(command);
            DataTable dt = new DataTable();
            da.Fill(dt);

            command.Dispose();
            sqlite_conn.Close();
            mutex.ReleaseMutex();
            return dt;
      }
      public long? ExecuteAndId(SQLiteCommand command)
      {
            var resId = (long)command.ExecuteScalar();
            command.Dispose();
            sqlite_conn.Close();
            mutex.ReleaseMutex();
            return resId;
      }
      public bool Execute(SQLiteCommand command)
      {
            command.ExecuteNonQuery();
            command.Dispose();
            sqlite_conn.Close();
            mutex.ReleaseMutex();
            return true;
      }
}