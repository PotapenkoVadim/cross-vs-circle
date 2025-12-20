internal class GameState: StateManager {
  private const int BOARD_SIZE = 10;
  public int BoardSize => BOARD_SIZE;

  private const int MAX_MOVES = 5;
  public int MaxMoves => MAX_MOVES;

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

  private Turn _turn;
  public Turn Turn
  {
    get => _turn;
    set => SetField(ref _turn, value);
  }

  private int _playerScore;
  public int PlayerScore {
    get => _playerScore;
    set => SetField(ref _playerScore, value);
  }

  private int _aiScore;
  public int AiScore {
    get => _aiScore;
    set => SetField(ref _aiScore, value);
  }

  private int _moves;
  public int Moves {
    get => _moves;
    set => SetField(ref _moves, value);
  }
}