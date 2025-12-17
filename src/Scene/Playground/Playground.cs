internal class Playground : Scene
{
  private readonly AppState _state;

  public Playground(AppState state) {
    _state = state;
    if (_state.GameState == null) {
      _state.GameState = new GameState();
    }
    
    InitializeGameState();
  }
  public override void HandleUserInput(InputKeys? userInput)
  {
    return;
  }

  public override void Render()
  {
    if (_state.GameState is not null && _state.GameState.Board is not null) {
      Console.WriteLine($"{_state.AppName} {_state.AppVersion}\n");
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
    return;
  }

  private void InitializeGameState() {
    if (_state.GameState == null) return;
    
    _state.GameState.Board = new CellState[_state.GameState.BoardSize, _state.GameState.BoardSize];

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

    if (pos.HasValue) {
      return pos.Value;
    } else {
      return (1, 1);
    }
  }
}