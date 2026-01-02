interface ILoader<T> where T: StateManager
{
  public void Save(T state);
  public List<Dictionary<string, T>> Load();
  public void DeleteSave(string date);
}