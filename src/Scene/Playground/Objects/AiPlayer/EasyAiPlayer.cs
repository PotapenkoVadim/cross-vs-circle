// INFO: algorithm finds neighbor cells and select empty cell if exists otherwise random cell
internal class EasyAiPlayer : IAiPlayer
{
  public void MakeMove(GameState state)
  {
    var neighbors = GameBoard.GetNeighbors(
      state.AiPosition.x, 
      state.AiPosition.y,
      GameState.BoardSize
    );
    
    var emptyCells = neighbors.Where(cell => 
      GameBoard.IsEmpty(state.Board!, cell.x, cell.y, GameState.BoardSize)
    ).ToList();

    var ownCells = neighbors.Where(cell => 
      GameBoard.IsOwnCell(state.Board!, cell.x, cell.y, CellState.Circle, GameState.BoardSize)
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
      state.Turn = Turn.Player;
      state.Moves = 0;
      return;
    }

    state.AiPosition = targetCell.Value;

    if (GameBoard.IsEmpty(state.Board!, targetCell.Value.x, targetCell.Value.y, GameState.BoardSize))
    {
      GameBoard.SetCell(state.Board!, targetCell.Value.x, targetCell.Value.y, CellState.Circle, GameState.BoardSize);
      state.AiScore++;
    }

    state.Moves++;
  }
}