using Microsoft.Data.Sqlite;

internal class DataBaseManager: IDisposable
{
  private readonly SqliteConnection _connection;
  private bool _dispose = false;

  public DataBaseManager(string dbPath)
  {
    var directory = Path.GetDirectoryName(dbPath);
    if (!string.IsNullOrWhiteSpace(directory))
    {
      Directory.CreateDirectory(directory);
    }

    var connectionString = new SqliteConnectionStringBuilder
    {
      DataSource = dbPath,
      Mode = SqliteOpenMode.ReadWriteCreate
    }.ToString();

    _connection = new SqliteConnection(connectionString);
    _connection.Open();

    ExecuteNonQuery("PRAGMA foreign_key = ON;");
  }

  public int ExecuteNonQuery(string query, Dictionary<string, object>? parameters = null)
  {
    using var command = CreateCommand(query, parameters);

    return command.ExecuteNonQuery();
  }

  public object? ExecuteScalar(string query, Dictionary<string, object>? parameters = null)
  {
    using var command = CreateCommand(query, parameters);

    return command.ExecuteScalar();
  }

  public IEnumerable<T> Select<T>(
    string query,
    Func<SqliteDataReader, T> mapper,
    Dictionary<string, object>? parameters = null
  )
  {
    using var command = CreateCommand(query, parameters);
    using var reader = command.ExecuteReader();
    while (reader.Read())
    {
      yield return mapper(reader);
    }
  }

  public int GetVersion() => Convert.ToInt32(ExecuteScalar("PRAGMA user_version"));

  public void SetVersion(int version) => ExecuteNonQuery($"PRAGMA user_version = {version}");

  private SqliteCommand CreateCommand(string query, Dictionary<string, object>? parameters)
  {
    var command = _connection.CreateCommand();
    command.CommandText = query;

    if (parameters != null)
    {
      foreach (var param in parameters)
      {
        command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
      }
    }

    return command;
  }

  public void Dispose()
  {
    if (_dispose) return;

    _connection?.Close();
    _connection?.Dispose();
    _dispose = true;
    GC.SuppressFinalize(this);
  }
}