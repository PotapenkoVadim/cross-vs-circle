internal class Playground : Scene
{
  private readonly AppState _state;
  private readonly AiPlayer _aiPlayer;

  public Playground(AppState state) {
    _state = state;
    _state.GameState ??= new GameState();
    _aiPlayer = new AiPlayer();
    
    InitializeGameState();
  }
  public override void HandleUserInput(InputKeys? userInput)
  {
    if (_state.GameState == null || _state.GameState.Board == null)
      return;

    if (IsGameFinished()) {
      if (userInput == InputKeys.Decline)
        _state.CurrentScene = AppScenes.Menu;

      return;
    }

    if (_state.GameState.Turn != Turn.Player)
      return;

    if (userInput == null)
      return;

    switch (userInput)
    {
      case InputKeys.Up:
        MovePlayerPosition(-1, 0);
        break;
      case InputKeys.Down:
        MovePlayerPosition(1, 0);
        break;
      case InputKeys.Left:
        MovePlayerPosition(0, -1);
        break;
      case InputKeys.Right:
        MovePlayerPosition(0, 1);
        break;
      case InputKeys.Decline:
        _state.CurrentScene = AppScenes.Menu;
        break;
    }
  }

  private void MovePlayerPosition(int deltaX, int deltaY)
  {
    if (_state.GameState == null || _state.GameState.Board == null)
      return;

    int newX = _state.GameState.PlayerPosition.x + deltaX;
    int newY = _state.GameState.PlayerPosition.y + deltaY;

    if (
      GameBoard.IsValidPosition(newX, newY, _state.GameState.BoardSize) &&
      !GameBoard.IsOpponentCell(_state.GameState.Board, newX, newY, CellState.Cross, _state.GameState.BoardSize) &&
      _state.GameState.Moves < _state.GameState.MaxMoves
    )
    {
      _state.GameState.PlayerPosition = (newX, newY);
      
      if (GameBoard.IsEmpty(_state.GameState.Board, newX, newY, _state.GameState.BoardSize))
      {
        GameBoard.SetCell(_state.GameState.Board, newX, newY, CellState.Cross, _state.GameState.BoardSize);
        _state.GameState.PlayerScore++;
        FloodFillIsolatedCells(CellState.Cross, () =>_state.GameState.PlayerScore += 1);
      }
      
      _state.GameState.Moves++;
      
      if (_state.GameState.Moves >= _state.GameState.MaxMoves)
      {
        _state.GameState.Turn = Turn.AI;
        _state.GameState.Moves = 0;
      }
    }
  }

  public override void Render()
  {
    if (_state.GameState is not null && _state.GameState.Board is not null) {
      if (IsGameFinished())
      {
        RenderGameFinished();
        return;
      }

      Console.WriteLine($"{_state.AppName} {_state.AppVersion}\n");
      Console.WriteLine($"Player cells: {_state.GameState.PlayerScore}");
      Console.WriteLine($"AI cells: {_state.GameState.AiScore}\n");
      GameBoard.RenderBoard(
        _state.GameState.Board,
        _state.GameState.PlayerPosition,
        _state.GameState.AiPosition,
        _state.GameState.BoardSize
      );
    }
  }

  private void RenderGameFinished()
  {
    if (_state.GameState == null)
      return;

    Console.WriteLine($"{_state.AppName} {_state.AppVersion}\n");
    Console.WriteLine("           GAME FINISHED!\n");
    
    Console.WriteLine($"Player score: {_state.GameState.PlayerScore}");
    Console.WriteLine($"AI score: {_state.GameState.AiScore}\n");

    if (_state.GameState.PlayerScore > _state.GameState.AiScore)
    {
      Console.WriteLine("Player wins!");
    } else if (_state.GameState.AiScore > _state.GameState.PlayerScore) {
      Console.WriteLine("AI wins!");
    } else {
      Console.WriteLine("It's a draw!");
    }

    Console.WriteLine("\nPress ESC to return to menu");
  }

  public override void Update()
  {
    if (_state.GameState == null || _state.GameState.Board == null)
      return;

    if (IsGameFinished()) return;

    if (_state.GameState.Turn == Turn.AI && _state.GameState.Moves < _state.GameState.MaxMoves)
    {
      // TODO: select ai level: _aiPlayer.MakeEasyMove(_state.GameState);
      _aiPlayer.MakeHardMove(_state.GameState);
      FloodFillIsolatedCells(CellState.Circle, () => _state.GameState.AiScore += 1);
    } else if (_state.GameState.Turn == Turn.AI && _state.GameState.Moves >= _state.GameState.MaxMoves)
    {
      _state.GameState.Turn = Turn.Player;
      _state.GameState.Moves = 0;
    }
  }

  private void InitializeGameState() {
    if (_state.GameState == null) return;
    
    _state.GameState.Board = new CellState[_state.GameState.BoardSize, _state.GameState.BoardSize];
    _state.GameState.Turn = Turn.Player;
    _state.GameState.Moves = 0;
    _state.GameState.PlayerScore = 0;
    _state.GameState.AiScore = 0;

    _state.GameState.PlayerPosition = GetRandomEmptyCell();
    GameBoard.SetCell(
      _state.GameState.Board,
      _state.GameState.PlayerPosition.x,
      _state.GameState.PlayerPosition.y,
      CellState.Cross,
      _state.GameState.BoardSize
    );

    _state.GameState.AiPosition = GetRandomEmptyCell();
    GameBoard.SetCell(
      _state.GameState.Board,
      _state.GameState.AiPosition.x,
      _state.GameState.AiPosition.y,
      CellState.Circle,
      _state.GameState.BoardSize
    );
  }
  
  private (int x, int y) GetRandomEmptyCell() {
    var pos = GameBoard.GetRandomEmptyCell(_state.GameState!.Board!, _state.GameState!.BoardSize);

    return pos!.Value;
  }

  private bool IsGameFinished() {
    var emptyCells = GameBoard.GetEmptyCells(_state.GameState!.Board!, _state.GameState!.BoardSize);
    if (emptyCells.Count == 0) {
      return true;
    }

    return false;
  }

  private void FloodFillIsolatedCells(CellState player, Action increaseScrore)
  {
    if (_state.GameState == null || _state.GameState.Board == null) return;

    for (int x = 0; x < _state.GameState.BoardSize; x++)
    {
      for (int y = 0; y < _state.GameState.BoardSize; y++)
      {
        if (GameBoard.IsOwnCell(
          _state.GameState.Board,
          x,
          y,
          player == CellState.Circle ? CellState.Cross : CellState.Circle,
          _state.GameState.BoardSize
        ))
        {
          MarkReachable(_state.GameState.Board, x, y, _state.GameState.BoardSize);
        }
      }
    }

    for (int x = 0; x < _state.GameState.BoardSize; x++)
    {
      for (int y = 0; y < _state.GameState.BoardSize; y++)
      {
        if (_state.GameState.Board[x, y] == CellState.Empty) {
          _state.GameState.Board[x, y] = player;
          increaseScrore();
        } else if (_state.GameState.Board[x, y] == CellState.Reachable)
        {
          _state.GameState.Board[x, y] = CellState.Empty;
        }
      }
    }
  }

  private void MarkReachable(CellState[,] board, int startX, int startY, int boardSize)
  {
    Stack<(int x, int y)> stack = new Stack<(int x, int y)>();
    stack.Push((startX, startY));

    while (stack.Count > 0)
    {
      var (x, y) = stack.Pop();

      var neighbors = GameBoard.GetNeighbors(x, y, boardSize);

      foreach (var neighbor in neighbors)
      {
        if (
          GameBoard.IsValidPosition(neighbor.x, neighbor.y, boardSize) &&
          GameBoard.IsEmpty(board, neighbor.x, neighbor.y, boardSize)
        ) {
          GameBoard.SetCell(board, neighbor.x, neighbor.y, CellState.Reachable, boardSize);
          stack.Push((neighbor.x, neighbor.y));
        }
      }
    }
  }
}