using System.ComponentModel;

sealed class App
{
  private const int FRAME_DELAY_MS = 160;
  private Scene? _scene;
  private IUserInputHandler _inputHandler;
  private AppState _state = new();

  public App()
  {
    Console.CursorVisible = false;
    _state.IsRunning = true;
    _inputHandler = new ConsoleInputHandler();
    _scene = SetScene(_state.CurrentScene);
    _state.PropertyChanged += OnStatePropertyChanged;
  }

  private void ProcessInput()
  {
    try
    {
      InputKeys? userInput = _inputHandler.InputHandler();
      _scene?.HandleUserInput(userInput);
    } catch
    {
      // TODO: handler exception
    }
  }

  private void Update()
  {
    try
    {
      _scene?.Update();
    } catch
    {
      // TODO: handle exception
    }
  }

  private void Render()
  {
    try
    {
      Console.Clear();
      _scene?.Render();
    } catch
    {
      // TODO: handle exception
    }
  }

  public void Run()
  {
    while (_state.IsRunning)
    {
      ProcessInput();
      Update();
      Render();

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
      AppScenes.Load => new Load(),
      AppScenes.Save => new Save(),
      AppScenes.Playground => new Playground(),
      _ => throw new ArgumentException("The scene could not be displayed correctly.")
    };
  }
}