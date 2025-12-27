// TODO: Necessary to implement factory for different AIs if the game is going to support different difficulty levels.
// TODO: AiPlayerFactory(GameLevels levels)
internal class AiPlayer
{
  private Stack<(int x, int y)>? _path = new();
  public void MakeEasyMove(GameState gameState)
  {
    var neighbors = GameBoard.GetNeighbors(
      gameState.AiPosition.x, 
      gameState.AiPosition.y,
      GameState.BoardSize
    );
    
    var emptyCells = neighbors.Where(cell => 
      GameBoard.IsEmpty(gameState.Board!, cell.x, cell.y, GameState.BoardSize)
    ).ToList();

    var ownCells = neighbors.Where(cell => 
      GameBoard.IsOwnCell(gameState.Board!, cell.x, cell.y, CellState.Circle, GameState.BoardSize)
    ).ToList();

    (int x, int y)? targetCell = null;
    Random random = new();

    if (emptyCells.Count > 0)
    {
      targetCell = emptyCells[random.Next(emptyCells.Count)];
    } else if (ownCells.Count > 0)
    {
      targetCell = ownCells[random.Next(ownCells.Count)];
    }

    if (!targetCell.HasValue)
    {
      gameState.Turn = Turn.Player;
      gameState.Moves = 0;
      return;
    }

    gameState.AiPosition = targetCell.Value;

    if (GameBoard.IsEmpty(gameState.Board!, targetCell.Value.x, targetCell.Value.y, GameState.BoardSize))
    {
      GameBoard.SetCell(gameState.Board!, targetCell.Value.x, targetCell.Value.y, CellState.Circle, GameState.BoardSize);
      gameState.AiScore++;
    }

    gameState.Moves++;
  }

  public void MakeHardMove(GameState gameState)
  {
    if (gameState.Moves == GameState.MaxMoves)
    {
      gameState.Turn = Turn.Player;
      gameState.Moves = 0;
      return;
    }

    if (ShouldInitPath(GameState.BoardSize, gameState.Board!))
    {
      _path = FindNearestEmptyCell(gameState);
    }

    if (_path != null && _path.Count > 0)
    {
      (int x, int y) nextMove = _path.Pop();

      gameState.AiPosition = nextMove;
      if (GameBoard.IsEmpty(gameState.Board!, nextMove.x, nextMove.y, GameState.BoardSize))
      {
        GameBoard.SetCell(gameState.Board!, nextMove.x, nextMove.y, CellState.Circle, GameState.BoardSize);
        gameState.AiScore++;
      }

      gameState.Moves++;
    } else
    {
      MakeEasyMove(gameState);
    }
  }

  private bool ShouldInitPath(int boardSize, CellState[,] board)
  {
    if (_path == null || _path.Count == 0) return true;

    var (x, y) = _path.Peek();

    return !GameBoard.IsOpponentCell(board, x, y, CellState.Circle, boardSize);
  }

  private Stack<(int x, int y)>? FindNearestEmptyCell(GameState gameState)
  {
    var queue = new Queue<(int x, int y)>();
    queue.Enqueue(gameState.AiPosition);

    var cameFrom = new Dictionary<(int x, int y), (int x, int y)>
    {
      [gameState.AiPosition] = gameState.AiPosition
    };

    (int dx, int dy)[] directions = [(-1, 0), (1, 0), (0, 1), (0, -1)];
    (int x, int y)? target = null;

    while (queue.Count > 0)
    {
      var current = queue.Dequeue();

      if (GameBoard.IsEmpty(gameState.Board!, current.x, current.y, GameState.BoardSize))
      {
        target = current;
        break;
      }

      foreach (var (dx, dy) in directions)
      {
        int nx = current.x + dx;
        int ny = current.y + dy;
        var neighbor = (nx, ny);

        if (
          nx >= 0 && nx < GameState.BoardSize &&
          ny >= 0 && ny < GameState.BoardSize &&
          !GameBoard.IsOpponentCell(gameState.Board!, nx, ny, CellState.Circle, GameState.BoardSize) &&
          !cameFrom.ContainsKey(neighbor)
        )
        {
          cameFrom[neighbor] = current;
          queue.Enqueue(neighbor);
        }
      }
    }

    if (target.HasValue)
    {
      return ReconstructPath(cameFrom, gameState.AiPosition, target.Value);
    }

    return null;
  }

  private static Stack<(int x, int y)>ReconstructPath(
    Dictionary<(int, int), (int, int)> cameFrom,
    (int x, int y) start,
    (int x, int y) end
  )
  {
    var path = new Stack<(int x, int y)>();
    var current = end;

    while (current != start)
    {
      path.Push(current);
      current = cameFrom[current];
    }

    return path;
  }
}