// INFO: shortest path algorithm. algorithm finds nearest empty cell and moves to the cell.
internal class MediumAiPlayer : IAiPlayer
{
  private Stack<(int x, int y)>? _path = new();

  public void MakeMove(GameState state)
  {
    if (state.Moves == GameState.MaxMoves)
    {
      state.Turn = Turn.Player;
      state.Moves = 0;
      return;
    }

    if (ShouldInitPath(GameState.BoardSize, state.Board!))
    {
      _path = FindNearestEmptyCell(state);
    }

    if (_path != null && _path.Count > 0)
    {
      (int x, int y) nextMove = _path.Pop();

      state.AiPosition = nextMove;
      if (GameBoard.IsEmpty(state.Board!, nextMove.x, nextMove.y, GameState.BoardSize))
      {
        GameBoard.SetCell(state.Board!, nextMove.x, nextMove.y, CellState.Circle, GameState.BoardSize);
        state.AiScore++;
      }

      state.Moves++;
    } else
    {
      var neighbors = GameBoard.GetNeighbors(
        state.AiPosition.x, 
        state.AiPosition.y,
        GameState.BoardSize
      );

      var ownCells = neighbors.Where(cell => 
        GameBoard.IsOwnCell(state.Board!, cell.x, cell.y, CellState.Circle, GameState.BoardSize)
      ).ToList();

      Random random = new();
      (int x, int y) targetCell = ownCells[random.Next(ownCells.Count)];
      state.AiPosition = targetCell;

      if (GameBoard.IsEmpty(state.Board!, targetCell.x, targetCell.y, GameState.BoardSize))
      {
        GameBoard.SetCell(state.Board!, targetCell.x, targetCell.y, CellState.Circle, GameState.BoardSize);
        state.AiScore++;
      }

      state.Moves++;
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