using Microsoft.Data.Sqlite;

internal class DataBaseManager: IDisposable
{
  private readonly SqliteConnection _connection;
  private const int TARGET_VERSION = 2;
  private const string DB_FILE = "database.db";
  private bool _dispose = false;

  private static DataBaseManager? _instance;

  public static DataBaseManager Instance
  {
    get
    {
      if (_instance is null)
      {
        string dbPath = Path.Combine(Directory.GetCurrentDirectory(), DB_FILE);
        _instance = new DataBaseManager(dbPath);
      }

      return _instance;
    }
  }

  private DataBaseManager(string dbPath)
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
    ApplyMigrations();
  }

  private void ApplyMigrations()
  {
    int currentVersion = GetVersion();

     if (currentVersion > TARGET_VERSION) throw new Exception($"Migration failed: current version is newer target");
    if (currentVersion == TARGET_VERSION) return;

    if (currentVersion < 1)
    {
      // MIGRATION: Create GameState table
      ExecuteNonQuery(@"
      CREATE TABLE IF NOT EXISTS GameState (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        created_at TEXT DEFAULT CURRENT_TIMESTAMP,
        board BLOB NOT NULL,
        player_x INTEGER NOT NULL CHECK(player_x >= 0 AND player_x < 10),
        player_y INTEGER NOT NULL CHECK(player_y >= 0 AND player_y < 10),
        ai_x INTEGER NOT NULL CHECK(ai_x >= 0 AND ai_x < 10),
        ai_y INTEGER NOT NULL CHECK(ai_y >= 0 AND ai_y < 10),
        turn INTEGER NOT NULL CHECK (turn IN (1, 0)),
        player_score INTEGER NOT NULL CHECK(player_score > 0),
        ai_score INTEGER NOT NULL CHECK(ai_score > 0)
      );
      ");
      SetVersion(1);
      currentVersion = 1;
    }

    if (currentVersion < 2)
    {
      // MIGRRATION: Add column moves to GameState table
      ExecuteNonQuery("ALTER TABLE GameState ADD COLUMN moves INTEGER DEFAULT 0;");
      SetVersion(2);
      currentVersion = 2;
    }

    if (currentVersion != TARGET_VERSION)
      throw new Exception($"Migration failed: current version {currentVersion}");
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