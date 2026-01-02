internal class SettingState: StateManager
{
  private DifficultyVariant _difficultyVariant;
  public DifficultyVariant DifficultyVariant
  {
    get => _difficultyVariant;
    set => SetField(ref _difficultyVariant, value);
  }
}