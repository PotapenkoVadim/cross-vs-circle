internal class GameState: StateManager {
  private const int BOARD_SIZE = 10;
  public int BoardSize => BOARD_SIZE;

  private CellState[,]? _board;
  public CellState[,]? Board {
    get => _board;
    set => SetField(ref _board, value);
  }

  private (int x, int y) _playerPosition;
  public (int x, int y) PlayerPosition {
    get => _playerPosition;
    set => SetField(ref _playerPosition, value);
  }

  private (int x, int y) _aiPosition;
  public (int x, int y) AiPosition {
    get => _aiPosition;
    set => SetField(ref _aiPosition, value);
  }
}