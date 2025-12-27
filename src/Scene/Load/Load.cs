internal class Load : Scene
{
  private readonly GameLoader _loader = new();
  private readonly AppState _state;
  private List<Dictionary<string, GameState>> _savingStates;

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
    }
  }

  public override void Render()
  {
    int index = 1;
    Console.WriteLine("=== LOADING GAME ===\n\n");
    
    foreach (var dictionary in _savingStates)
    {
      foreach (var (created, state) in dictionary)
      {
        DateTime saveTime = DateTime.Parse(created);
        string timeDisplay = GetRelativeTime(saveTime);

        Console.WriteLine($" {index}. [Save {timeDisplay}]");
        Console.WriteLine($"    Score: Player {state.PlayerScore} - {state.AiScore} AI");
        Console.WriteLine("    ----------------------------------");

        index++;
      }
    }

    Console.WriteLine("\n\n[ENTER]   confirm");
    Console.WriteLine("[ESC]     return to menu");
  }

  public override void Update()
  {
    return;
  }

  private string GetRelativeTime(DateTime date)
  {
    var ts = DateTime.Now - date;

    if (ts.TotalMinutes < 1) return "Just now";
    if (ts.TotalMinutes < 60) return $"{(int)ts.TotalMinutes}m ago";
    if (ts.TotalHours < 24) return $"{(int)ts.TotalHours}h ago";

    return date.ToShortDateString();
  }
}