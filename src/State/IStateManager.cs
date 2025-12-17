using System.ComponentModel;
using System.Runtime.CompilerServices;

public interface IStateManager: INotifyPropertyChanged {
  public new event PropertyChangedEventHandler PropertyChanged;

  public void OnPropertyChanged([CallerMemberName] string? propertyName = null);
  public bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null);
}