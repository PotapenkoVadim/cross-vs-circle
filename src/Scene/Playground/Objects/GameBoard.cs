internal static class GameBoard {
  public static void RenderBoard(
    CellState[,] board,
    (int x, int y)? playerPosition = null,
    (int x, int y)? aiPosition = null,
    int boardSize = 0
  )
  {
    Console.Write("   ");
    for (int col = 0; col < boardSize; col++)
    {
      Console.Write($"{col} ");
    }
    Console.WriteLine();

    for (int row = 0; row < boardSize; row++)
    {
      Console.Write($"{row}  ");

      for (int col = 0; col < boardSize; col++)
      {
        bool isPlayerPos = playerPosition.HasValue && 
                          playerPosition.Value.x == row && 
                          playerPosition.Value.y == col;
                          
        bool isAIPos = aiPosition.HasValue && 
                      aiPosition.Value.x == row && 
                      aiPosition.Value.y == col;

        ConsoleColor originalColor = Console.ForegroundColor;

        string symbol;
        string blinkStart = "\u001b[5m";
        string blinkEnd = "\u001b[25m";
        
        if (board[row, col] == CellState.Cross && isPlayerPos)
        {
          Console.ForegroundColor = ConsoleColor.Red;
          symbol = $"{blinkStart}X{blinkEnd}";
        }
        else if (board[row, col] == CellState.Circle && isAIPos)
        {
          Console.ForegroundColor = ConsoleColor.Blue;
          symbol = $"{blinkStart}O{blinkEnd}";
        }
        else
        {
          symbol = board[row, col] switch
          {
            CellState.Empty => ".",
            CellState.Cross => "X",
            CellState.Circle => "O",
            _ => "."
          };
        }

        Console.Write($"{symbol} ");
        Console.ForegroundColor = originalColor;
      }

      Console.WriteLine();
    }
  }

  public static bool IsValidPosition(int x, int y, int boardSize)
  {
    return x >= 0 && x < boardSize && y >= 0 && y < boardSize;
  }

  public static CellState GetCell(CellState[,] board, int x, int y, int boardSize)
  {
    if (!IsValidPosition(x, y, boardSize))
    {
      return CellState.Empty;
    }
    return board[x, y];
  }

  public static void SetCell(CellState[,] board, int x, int y, CellState state, int boardSize)
  {
    if (IsValidPosition(x, y, boardSize))
    {
      board[x, y] = state;
    }
  }

  public static bool IsEmpty(CellState[,] board, int x, int y, int boardSize)
  {
    return GetCell(board, x, y, boardSize) == CellState.Empty;
  }

  public static bool IsOwnCell(CellState[,] board, int x, int y, CellState player, int boardSize)
  {
    return GetCell(board, x, y, boardSize) == player;
  }

  public static bool IsOpponentCell(CellState[,] board, int x, int y, CellState player, int boardSize)
  {
    CellState opponent = player == CellState.Cross ? CellState.Circle : CellState.Cross;
    return GetCell(board, x, y, boardSize) == opponent;
  }

  public static List<(int x, int y)> GetNeighbors(int x, int y, int boardSize)
  {
    var neighbors = new List<(int, int)>();
    
    if (IsValidPosition(x - 1, y, boardSize))
      neighbors.Add((x - 1, y));
    if (IsValidPosition(x + 1, y, boardSize))
      neighbors.Add((x + 1, y));
    if (IsValidPosition(x, y - 1, boardSize))
      neighbors.Add((x, y - 1));
    if (IsValidPosition(x, y + 1, boardSize))
      neighbors.Add((x, y + 1));
    
    return neighbors;
  }

  public static List<(int x, int y)> GetEmptyCells(CellState[,] board, int boardSize)
  {
    var emptyCells = new List<(int, int)>();
    
    for (int row = 0; row < boardSize; row++)
    {
      for (int col = 0; col < boardSize; col++)
      {
        if (IsEmpty(board, row, col, boardSize))
        {
          emptyCells.Add((row, col));
        }
      }
    }
    
    return emptyCells;
  }

  public static (int x, int y)? GetRandomEmptyCell(CellState[,] board, int boardSize)
  {
    var emptyCells = GetEmptyCells(board, boardSize);
    
    if (emptyCells.Count == 0)
    {
      return null;
    }
    
    Random random = new Random();
    int index = random.Next(emptyCells.Count);
    return emptyCells[index];
  }
}