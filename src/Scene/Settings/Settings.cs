internal class Settings(AppState state, ILoader<SettingState> loader) : Scene
{
  private readonly AppState _state = state;
  private readonly ILoader<SettingState> _loader = loader;

  public override void HandleUserInput(InputKeys? userInput)
  {
    ChangeDifficulty(userInput);
    
    switch (userInput)
    {
      case InputKeys.Decline:
        _state.CurrentScene = AppScenes.Menu;
        break;
      case InputKeys.Accept:
        SaveDifficulty();
        break;
    }
  }

  public override void Render()
  {
    if (_state.SettingState is null) return;

    string difficultyText = _state.SettingState.DifficultyVariant == DifficultyVariant.Medium
      ? "medium"
      : "easy";

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
      int selectedDifficulty = (int) _state.SettingState!.DifficultyVariant;
      selectedDifficulty ^= 1;
      _state.SettingState.DifficultyVariant = (DifficultyVariant)selectedDifficulty;
    }
  }

  private void SaveDifficulty()
  {
    _loader.Save(_state.SettingState!);
    var settingsState = _loader.Load().First();
    _state.SettingState = new SettingState
    {
      DifficultyVariant = settingsState["settings"].DifficultyVariant
    };
    _state.CurrentScene = AppScenes.Menu;
  }
}