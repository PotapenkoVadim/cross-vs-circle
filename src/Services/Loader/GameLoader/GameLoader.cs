using System.Runtime.InteropServices;
using Microsoft.Data.Sqlite;

internal class GameLoader: ILoader<GameState>
{
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

    DataBaseManager.Instance.ExecuteNonQuery(@"
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
    IEnumerable<Dictionary<string, GameState>> results = DataBaseManager.Instance.Select(query, MapRowToGameState);

    return [.. results];
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
    DataBaseManager.Instance.ExecuteNonQuery(query, parameters);
  }
}
