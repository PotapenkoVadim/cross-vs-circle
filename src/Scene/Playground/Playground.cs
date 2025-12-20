internal class Playground : Scene
{
  private readonly AppState _state;

  public Playground(AppState state) {
    _state = state;
    _state.GameState ??= new GameState();
    
    InitializeGameState();
  }
  public override void HandleUserInput(InputKeys? userInput)
  {
    if (_state.GameState == null || _state.GameState.Board == null)
      return;

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

  public override void Update()
  {
    if (_state.GameState == null || _state.GameState.Board == null)
      return;

    if (IsGameFinished()) {
      // _state.GameState.GamePhase = GamePhase.Finished;
      _state.CurrentScene = AppScenes.Menu;
      return;
    }

    if (_state.GameState.Turn == Turn.AI && _state.GameState.Moves < _state.GameState.MaxMoves)
    {
      MakeAIMove();
    } else if (_state.GameState.Turn == Turn.AI && _state.GameState.Moves >= _state.GameState.MaxMoves)
    {
      _state.GameState.Turn = Turn.Player;
      _state.GameState.Moves = 0;
    }
  }

  private void MakeAIMove()
  {
    if (_state.GameState == null || _state.GameState.Board == null)
      return;

    var neighbors = GameBoard.GetNeighbors(
      _state.GameState.AiPosition.x, 
      _state.GameState.AiPosition.y, 
      _state.GameState.BoardSize
    );
    
    var emptyCells = neighbors.Where(cell => 
      GameBoard.IsEmpty(_state.GameState!.Board!, cell.x, cell.y, _state.GameState.BoardSize)
    ).ToList();

    var ownCells = neighbors.Where(cell => 
      GameBoard.IsOwnCell(_state.GameState!.Board!, cell.x, cell.y, CellState.Circle, _state.GameState.BoardSize)
    ).ToList();

    (int x, int y)? targetCell = null;
    Random random = new Random();

    if (emptyCells.Count > 0)
    {
      targetCell = emptyCells[random.Next(emptyCells.Count)];
    } else if (ownCells.Count > 0)
    {
      targetCell = ownCells[random.Next(ownCells.Count)];
    }

    if (!targetCell.HasValue)
    {
      _state.GameState.Turn = Turn.Player;
      _state.GameState.Moves = 0;
      return;
    }

    _state.GameState.AiPosition = targetCell.Value;

    if (GameBoard.IsEmpty(_state.GameState.Board, targetCell.Value.x, targetCell.Value.y, _state.GameState.BoardSize))
    {
      GameBoard.SetCell(_state.GameState.Board, targetCell.Value.x, targetCell.Value.y, CellState.Circle, _state.GameState.BoardSize);
      _state.GameState.AiScore++;
    }

    _state.GameState.Moves++;
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
}