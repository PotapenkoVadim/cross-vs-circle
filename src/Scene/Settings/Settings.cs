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
    Console.WriteLine("=== SETTINGS ===");
  }

  public override void Update()
  {
    return;
  }
}