internal class Menu : Scene
{
  private const string START_MENU_TEXT = "   START   ";
  private const string LOAD_MENU_TEXT = "   LOAD   ";
  private const string SAVE_MENU_TEXT = "   SAVE   ";
  private const string EXIT_MENU_TEXT = "   EXIT   ";
  private const string SELECTED_SYMBOL = "#";
  private const string NOT_SELECTED_SYMBOL = " ";

  private static readonly Dictionary<MenuVariant, string> MenuTexts = new()
  {
    { MenuVariant.START, START_MENU_TEXT },
    { MenuVariant.SAVE, SAVE_MENU_TEXT },
    { MenuVariant.LOAD, LOAD_MENU_TEXT },
    { MenuVariant.EXIT, EXIT_MENU_TEXT }
  };

  private static readonly MenuVariant[] MenuOrder = new[]
  {
    MenuVariant.START,
    MenuVariant.SAVE,
    MenuVariant.LOAD,
    MenuVariant.EXIT
  };

  private readonly AppState _state;

  public Menu(AppState state)
  {
    _state = state;
  }
  
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
        _state.CurrentScene = AppScenes.Playground;
        break;
      case MenuVariant.SAVE:
        _state.CurrentScene = AppScenes.Save;
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