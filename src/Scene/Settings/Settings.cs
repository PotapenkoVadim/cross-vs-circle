internal class Settings : Scene
{
  private readonly AppState _state;
  private readonly ILoader<SettingState> _loader;
  private int _selectedDifficulty = 0;
  public Settings(AppState state, ILoader<SettingState> loader)
  {
    _state = state;
    _loader = loader;

    if (_state.SettingState?.DifficultyVariant is not null)
      _selectedDifficulty = (int)_state.SettingState.DifficultyVariant;
  }

  public override void HandleUserInput(InputKeys? userInput)
  {
    ChangeDifficulty(userInput);
    
    switch (userInput)
    {
      case InputKeys.Decline:
        _state.CurrentScene = AppScenes.Menu;
        break;
    }
  }

  public override void Render()
  {
    string difficultyText = _selectedDifficulty == 1 ? "medium" : "easy";
    Console.WriteLine("=== SETTINGS ===\n\n");
    Console.WriteLine($"Select difficulty: {difficultyText}");

    Console.WriteLine("\n\n[\u2190 \u2192]      change difficulty");
    Console.WriteLine("[ENTER]    confirm");
    Console.WriteLine("[ESC]      return to menu");
  }

  public override void Update()
  {
    return;
  }

  private void ChangeDifficulty(InputKeys? userInput)
  {
    if (userInput is InputKeys.Left or InputKeys.Right)
    {
      _selectedDifficulty ^= 1;
    }
  }
}