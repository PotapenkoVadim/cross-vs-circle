internal class Save(AppState state) : Scene
{
  private readonly GameLoader _loader = new();
  private readonly AppState _state = state;

  public override void HandleUserInput(InputKeys? userInput)
  {
    if (_state.GameState == null)
      throw new ArgumentException("Incorrect loading of game state.");

    switch (userInput)
    {
      case InputKeys.Decline:
        _state.CurrentScene = AppScenes.Menu;
        break;

      case InputKeys.QuickSave:
        _state.CurrentScene = AppScenes.Playground;
        break;

      case InputKeys.Accept:
        _loader.Save(_state.GameState);
        _state.CurrentScene = AppScenes.Playground;
        break;
    }
  }

  public override void Render()
  {
    if (_state.GameState == null)
      throw new ArgumentException("Incorrect loading of game state.");

    string currentTurn = _state.GameState.Turn == Turn.Player ? "Player" : "AI";

    Console.WriteLine("=== SAVING GAME ===");
    Console.WriteLine("\nDo you want to save the current progress?\n\n");
    Console.WriteLine($"Player Score: {_state.GameState.PlayerScore}");
    Console.WriteLine($"AI Score: {_state.GameState.AiScore}");
    Console.WriteLine($"Current turn: {currentTurn}\n\n");

    Console.WriteLine("[ENTER]   confirm and return to game");
    Console.WriteLine("[F5]      return to game");
    Console.WriteLine("[ESC]     return to menu");
  }

  public override void Update()
  {
    if (_state.GameState == null)
      throw new Exception("Incorrect loading of game state.");
  }
}