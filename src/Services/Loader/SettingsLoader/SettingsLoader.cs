using Microsoft.Data.Sqlite;

internal class SettingsLoader: ILoader<SettingState>
{
  public void Save(SettingState state)
  {
    throw new Exception("");
  }

  public List<Dictionary<string, SettingState>> Load()
  {
    string query = "SELECT difficulty_variant FROM Settings;";
    IEnumerable<Dictionary<string, SettingState>> results = DataBaseManager.Instance.Select(query, MapRowToSettingsState);

    return [.. results];
  }

  public void DeleteSave(string data)
  {
    throw new Exception("");
  }

  private Dictionary<string, SettingState> MapRowToSettingsState(SqliteDataReader reader)
  {
    var state = new SettingState
    {
      DifficultyVariant = (DifficultyVariant)reader.GetInt32(reader.GetOrdinal("difficulty_variant"))
    };

    return new Dictionary<string, SettingState> {{"settings", state}};
  }
}