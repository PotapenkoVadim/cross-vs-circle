internal class Menu(AppState state) : Scene
{
  private const string START_MENU_TEXT = "   NEW GAME   ";
  private const string LOAD_MENU_TEXT = "   LOAD GAME   ";
  private const string SETTINGS_MENU_TEXT = "   SETTINGS   ";
  private const string EXIT_MENU_TEXT = "   EXIT   ";
  private const string SELECTED_SYMBOL = "#";
  private const string NOT_SELECTED_SYMBOL = " ";

  private static readonly Dictionary<MenuVariant, string> MenuTexts = new()
  {
    { MenuVariant.START, START_MENU_TEXT },
    { MenuVariant.LOAD, LOAD_MENU_TEXT },
    { MenuVariant.SETTINGS, SETTINGS_MENU_TEXT },
    { MenuVariant.EXIT, EXIT_MENU_TEXT }
  };

  private static readonly MenuVariant[] MenuOrder = new[]
  {
    MenuVariant.START,
    MenuVariant.LOAD,
    MenuVariant.SETTINGS,
    MenuVariant.EXIT
  };

  private readonly AppState _state = state;

  public override void HandleUserInput(InputKeys? userInput)
  {
    if (userInput == InputKeys.Accept) {
      SelectMenuItem();
      return;
    }

    SwitchMenuItem(userInput);
  }

  public override void Render()
  {
    Console.WriteLine($"{_state.AppName} {_state.AppVersion}\n");
    Console.WriteLine(GetMenuText());
  }

  public override void Update()
  {
    return;
  }

  private string GetMenuText()
  {
    var lines = MenuOrder.Select(variant =>
    {
      string symbol = variant == _state.SelectedMenu 
        ? SELECTED_SYMBOL 
        : NOT_SELECTED_SYMBOL;

      return $"{symbol} {MenuTexts[variant]}";
    });

    return string.Join("\n", lines) + "\n";
  }

  private void SwitchMenuItem(InputKeys? userInput)
  {
    int currentVariant = (int)_state.SelectedMenu;
    int length = MenuOrder.Length - 1;
    currentVariant = userInput switch
    {
      InputKeys.Down => currentVariant + 1 > length ? 0 : currentVariant + 1,
      InputKeys.Up => currentVariant - 1 < 0 ? length : currentVariant - 1,
      _ => currentVariant
    };

    _state.SelectedMenu = (MenuVariant)currentVariant;
  }

  private void SelectMenuItem()
  {
    switch (_state.SelectedMenu)
    {
      case MenuVariant.START:
        _state.GameState = null;
        _state.CurrentScene = AppScenes.Playground;
        break;
      case MenuVariant.SETTINGS:
        _state.CurrentScene = AppScenes.Settings;
        break;
      case MenuVariant.LOAD:
        _state.CurrentScene = AppScenes.Load;
        break;
      case MenuVariant.EXIT:
        _state.IsRunning = false;
        break;
      default:
        throw new ArgumentException($"Unknown menu variant: {_state.SelectedMenu}", nameof(_state.SelectedMenu));
    }
  }
}