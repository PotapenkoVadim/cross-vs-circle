using System.Runtime.InteropServices;
using Microsoft.Data.Sqlite;

internal class GameLoader
{
  private const string DB_FILE = "database.db";
  private const int TARGET_VERSION = 2;
  private readonly DataBaseManager _dbManager;
  public GameLoader()
  {
    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), DB_FILE);
    _dbManager = new DataBaseManager(fullPath);

    ApplyMigrations();
  }

  public void Save(GameState state)
  {
    if (state.Board is null)
      throw new ArgumentException("Cannot save a state with a null board");
    
    var parameters = new Dictionary<string, object>
    {
      {"@board", MatrixToBlob(state.Board)},
      {"@player_x", state.PlayerPosition.x},
      {"@player_y", state.PlayerPosition.y},
      {"@ai_x", state.AiPosition.x},
      {"@ai_y", state.AiPosition.y},
      {"@turn", state.Turn},
      {"@player_score", state.PlayerScore},
      {"@ai_score", state.AiScore},
      {"@moves", state.Moves}
    };

    _dbManager.ExecuteNonQuery(@"
    INSERT INTO GameState (
      board, player_x, player_y, ai_x, ai_y, turn, player_score, ai_score, moves
    ) VALUES (
      @board, @player_x, @player_y, @ai_x, @ai_y, @turn, @player_score, @ai_score, @moves
    );
    ", parameters);
  }

  public List<Dictionary<string, GameState>> Load()
  {
    string query = "SELECT board, player_x, player_y, ai_x, ai_y, turn, player_score, ai_score, moves, created_at FROM GameState";
    IEnumerable<Dictionary<string, GameState>> results = _dbManager.Select(query, MapRowToGameState);

    return [.. results];
  }

  private void ApplyMigrations()
  {
    int currentVersion = _dbManager.GetVersion();

     if (currentVersion > TARGET_VERSION) throw new Exception($"Migration failed: current version is newer target");
    if (currentVersion == TARGET_VERSION) return;

    if (currentVersion < 1)
    {
      // MIGRATION: Create GameState table
      _dbManager.ExecuteNonQuery(@"
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
      _dbManager.SetVersion(1);
      currentVersion = 1;
    }

    if (currentVersion < 2)
    {
      // MIGRRATION: Add column moves to GameState table
      _dbManager.ExecuteNonQuery("ALTER TABLE GameState ADD COLUMN moves INTEGER;");
      _dbManager.SetVersion(2);
      currentVersion = 2;
    }

    if (currentVersion != TARGET_VERSION)
      throw new Exception($"Migration failed: current version {currentVersion}");
  }

  private byte[] MatrixToBlob(CellState[,] matrix)
  {
    int size = GameState.BoardSize * GameState.BoardSize;
    ReadOnlySpan<CellState> span = MemoryMarshal.CreateReadOnlySpan(ref matrix[0, 0], size);

    return MemoryMarshal.AsBytes(span).ToArray();
  }

  private Dictionary<string, GameState> MapRowToGameState(SqliteDataReader reader)
  {
    byte[] blob = (byte[])reader["board"];
    CellState[,] board = new CellState[GameState.BoardSize, GameState.BoardSize];

    int bytesToCopy = Math.Min(blob.Length, GameState.BoardSize * GameState.BoardSize * sizeof(CellState));
    Buffer.BlockCopy(blob, 0, board, 0, bytesToCopy);

    var state = new GameState
    {
      Board = board,
      PlayerPosition = (
        reader.GetInt32(reader.GetOrdinal("player_x")),
        reader.GetInt32(reader.GetOrdinal("player_y"))
      ),
      AiPosition = (
        reader.GetInt32(reader.GetOrdinal("ai_x")),
        reader.GetInt32(reader.GetOrdinal("ai_y"))
      ),
      Turn = (Turn)(reader.GetInt32(reader.GetOrdinal("turn"))),
      PlayerScore = reader.GetInt32(reader.GetOrdinal("player_score")),
      AiScore = reader.GetInt32(reader.GetOrdinal("ai_score")),
      Moves = reader.GetInt32(reader.GetOrdinal("moves"))
    };

    string createdAt = reader.GetString(reader.GetOrdinal("created_at"));

    return new Dictionary<string, GameState>{{createdAt, state}};
  }

  public void DeleteSave(string date)
  {
    string query = "DELETE FROM GameState WHERE created_at = @created";
    var parameters = new Dictionary<string, object> {{"@created", date}};
    _dbManager.ExecuteNonQuery(query, parameters);
  }
}
