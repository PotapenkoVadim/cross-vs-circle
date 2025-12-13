using System.ComponentModel;
using System.Runtime.CompilerServices;

public interface IStateManager: INotifyPropertyChanged {
  public new event PropertyChangedEventHandler PropertyChanged;

  protected void OnPropertyChanged([CallerMemberName] string? propertyName = null);
  protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null);
}