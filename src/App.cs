using System.ComponentModel;

sealed class App
{
  private const int FRAME_DELAY_MS = 160;
  private Scene? _scene;
  private readonly IUserInputHandler _inputHandler;
  private readonly AppState _state = new();
  private readonly ErrorLogger _errorLogger = new();
  private readonly ILoader<SettingState> _settingLoader = new SettingsLoader();

  public App()
  {
    Console.CursorVisible = false;
    _state.IsRunning = true;
    _inputHandler = new ConsoleInputHandler();
    _scene = SetScene(_state.CurrentScene);
    InitializeSettings();
    _state.PropertyChanged += OnStatePropertyChanged;
  }

  private void ProcessInput()
  {
    InputKeys? userInput = _inputHandler.InputHandler();
    _scene?.HandleUserInput(userInput);
  }

  private void Update()
  {
    _scene?.Update();
  }

  private void Render()
  {
    Console.Clear();
    _scene?.Render();
  }

  public void Run()
  {
    while (_state.IsRunning)
    {
      try {
        ProcessInput();
        Update();
        Render();
      } catch (Exception ex) {
        _errorLogger.LogError(ex.ToString());
        _state.IsRunning = false;
      }

      Thread.Sleep(FRAME_DELAY_MS);
    }

    Console.Clear();
    Console.WriteLine("\nPress any key to exit...");
    Console.ReadKey();
  }

  private void OnStatePropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(AppState.CurrentScene))
    {
      _scene = SetScene(_state.CurrentScene);
    }
  }

  private Scene SetScene(AppScenes sceneVariant)
  {
    return sceneVariant switch
    {
      AppScenes.Menu => new Menu(_state),
      AppScenes.Load => new Load(_state),
      AppScenes.Save => new Save(_state),
      AppScenes.Playground => new Playground(_state),
      AppScenes.Settings => new Settings(_state, _settingLoader),
      _ => throw new ArgumentException("The scene could not be displayed correctly.")
    };
  }

  private void InitializeSettings()
  {
    var settingsState = _settingLoader.Load().First();
    _state.SettingState = new SettingState
    {
      DifficultyVariant = settingsState["settings"].DifficultyVariant
    };
  }
}