internal class Settings : Scene
{
  private readonly AppState _state;
  public Settings(AppState state)
  {
    _state = state;
  }

  public override void HandleUserInput(InputKeys? userInput)
  {
    switch (userInput)
    {
      case InputKeys.Decline:
        _state.CurrentScene = AppScenes.Menu;
        break;
    }
  }

  public override void Render()
  {
    Console.WriteLine("=== SETTINGS ===\n\n");
    Console.WriteLine("Select difficulty: easy");

    Console.WriteLine("\n\n[\u2190 \u2192]      change difficulty");
    Console.WriteLine("[ENTER]    confirm");
    Console.WriteLine("[ESC]      return to menu");
  }

  public override void Update()
  {
    return;
  }
}