internal class Menu : Scene
{
  private const string START_MENU_TEXT = "   START   ";
  private const string LOAD_MENU_TEXT = "   LOAD   ";
  private const string SAVE_MENU_TEXT = "   SAVE   ";
  private const string EXIT_MENU_TEXT = "   EXIT   ";
  private const string SELECTED_SYMBOL = "#";
  private const string NOT_SELECTED_SYMBOL = " ";

  private readonly AppState _state;

  public Menu(ref AppState state)
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
    return _state.SelectedMenu switch
    {
      MenuVariant.START => $"""
      {SELECTED_SYMBOL}{START_MENU_TEXT}
      {NOT_SELECTED_SYMBOL}{SAVE_MENU_TEXT}
      {NOT_SELECTED_SYMBOL}{LOAD_MENU_TEXT}
      {NOT_SELECTED_SYMBOL}{EXIT_MENU_TEXT}
      """,
      MenuVariant.SAVE => $"""
      {NOT_SELECTED_SYMBOL}{START_MENU_TEXT}
      {SELECTED_SYMBOL}{SAVE_MENU_TEXT}
      {NOT_SELECTED_SYMBOL}{LOAD_MENU_TEXT}
      {NOT_SELECTED_SYMBOL}{EXIT_MENU_TEXT}
      """,
      MenuVariant.LOAD => $"""
      {NOT_SELECTED_SYMBOL}{START_MENU_TEXT}
      {NOT_SELECTED_SYMBOL}{SAVE_MENU_TEXT}
      {SELECTED_SYMBOL}{LOAD_MENU_TEXT}
      {NOT_SELECTED_SYMBOL}{EXIT_MENU_TEXT}
      """,
      MenuVariant.EXIT => $"""
      {NOT_SELECTED_SYMBOL}{START_MENU_TEXT}
      {NOT_SELECTED_SYMBOL}{SAVE_MENU_TEXT}
      {NOT_SELECTED_SYMBOL}{LOAD_MENU_TEXT}
      {SELECTED_SYMBOL}{EXIT_MENU_TEXT}
      """,
      _ => throw new Exception("Menu loading error.")
    };
  }

  private void SwitchMenuItem(InputKeys? userInput)
  {
    int currentVariant = (int)_state.SelectedMenu;
    int length = Enum.GetValues<MenuVariant>().Length - 1;
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
    // TODO: implement select menu item
    return;
  }
}