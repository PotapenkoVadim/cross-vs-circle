internal class AppState: StateManager
{
  private MenuVariant _selectedMenu;

  public MenuVariant SelectedMenu
  {
    get => _selectedMenu;
    set => SetField(ref _selectedMenu, value);
  }

  private AppScenes _currentScene = AppScenes.Menu;

  public AppScenes CurrentScene
  {
    get => _currentScene;
    set => SetField(ref _currentScene, value);
  }

  private bool _isRunning;
  public bool IsRunning
  {
    get => _isRunning;
    set => SetField(ref _isRunning, value);
  }

  private GameState? _gameState;
  public GameState? GameState {
    get => _gameState;
    set => SetField(ref _gameState, value);
  }
}