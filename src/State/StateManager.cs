using System.ComponentModel;
using System.Runtime.CompilerServices;

internal abstract class StateManager: IStateManager
{
  private const string APP_NAME = "CROSS VS CIRCLE.";
  private const string APP_VERSION = "v0.0.1";
  public event PropertyChangedEventHandler? PropertyChanged;

  public string AppName => APP_NAME;
  public string AppVersion => APP_VERSION;

  public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  public bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
  {
    if (EqualityComparer<T>.Default.Equals(field, value))
      return false;

    field = value;
    OnPropertyChanged(propertyName);

    return true;
  }
}