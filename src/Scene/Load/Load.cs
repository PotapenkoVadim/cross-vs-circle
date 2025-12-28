internal class Load : Scene
{
  private readonly GameLoader _loader = new();
  private readonly AppState _state;
  private List<Dictionary<string, GameState>> _savingStates;

  private int _selectedItem = 1;

  public Load(AppState state)
  {
    _state = state;
    _savingStates = _loader.Load();
  }

  public override void HandleUserInput(InputKeys? userInput)
  {
    switch (userInput)
    {
      case InputKeys.Decline:
        _state.CurrentScene = AppScenes.Menu;
        break;

      case InputKeys.Up:
        _selectedItem = _selectedItem <= 1 ? _savingStates.Count : _selectedItem - 1;
        break;

      case InputKeys.Down:
        _selectedItem = _selectedItem >= _savingStates.Count ? 1 : _selectedItem + 1;
        break;

      case InputKeys.Accept:
        SelectSave();
        break;

      case InputKeys.Delete:
        DeleteSave();
        break;
    }
  }

  public override void Render()
  {
    Console.WriteLine("=== LOADING GAME ===\n\n");
    RenderSaves();
    Console.WriteLine("\n\n[\u2191\u2193]      select save");
    Console.WriteLine("[ENTER]   confirm and continue the game");
    Console.WriteLine("[DEL]     delete save");
    Console.WriteLine("[ESC]     return to menu");
  }

  public override void Update()
  {
    return;
  }

  private void RenderSaves()
  {
    if (_savingStates is null || _savingStates.Count == 0)
    {
      Console.WriteLine("No saves found");
      return;  
    }

    int index = 1;
    foreach (var dictionary in _savingStates)
    {
      foreach (var (created, state) in dictionary)
      {
        string symbol = index == _selectedItem ? "# " : $"{index}.";
        DateTime saveTime = DateTime.Parse(created);
        string timeDisplay = GetRelativeTime(saveTime);

        Console.WriteLine($" {symbol} [Save {timeDisplay}]");
        Console.WriteLine($"    Score: Player {state.PlayerScore} - {state.AiScore} AI");
        Console.WriteLine("    ----------------------------------");

        index++;
      }
    }
  }

  private string GetRelativeTime(DateTime date)
  {
    var ts = DateTime.UtcNow - date;

    if (ts.TotalMinutes < 1) return "Just now";
    if (ts.TotalMinutes < 60) return $"{(int)ts.TotalMinutes}m ago";
    if (ts.TotalHours < 24) return $"{(int)ts.TotalHours}h ago";

    return date.ToLocalTime().ToShortDateString();
  }

  private void SelectSave()
  {
    var selectedState = _savingStates[_selectedItem - 1];
    _state.GameState = selectedState.Values.First();
    _state.CurrentScene = AppScenes.Playground;
  }

  private void DeleteSave()
  {
    var selectedState = _savingStates[_selectedItem - 1];
    string date = selectedState.Keys.First();
    _loader.DeleteSave(date);
    _savingStates = _loader.Load();
  }
}