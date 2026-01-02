using Microsoft.Data.Sqlite;

internal class SettingsLoader: ILoader<SettingState>
{
  public void Save(SettingState state)
  {
    if (state is null)
      throw new ArgumentException("Cannot save a state with a null settings");
    
    var difficulty = state.DifficultyVariant == DifficultyVariant.Easy ? "easy" : "medium";
    var parameters = new Dictionary<string, object> {{"@difficulty_variant", difficulty}};

    DataBaseManager.Instance.ExecuteNonQuery(
      "UPDATE Settings SET difficulty_variant = @difficulty_variant WHERE id = 1;",
      parameters
    );
  }

  public List<Dictionary<string, SettingState>> Load()
  {
    string query = "SELECT difficulty_variant FROM Settings WHERE id = 1;";
    IEnumerable<Dictionary<string, SettingState>> results = DataBaseManager.Instance.Select(query, MapRowToSettingsState);

    return [.. results];
  }

  public void DeleteSave(string data)
  {
    throw new Exception("");
  }

  private Dictionary<string, SettingState> MapRowToSettingsState(SqliteDataReader reader)
  {
    var difficultyVariant = reader.GetString(reader.GetOrdinal("difficulty_variant")) == "easy"
      ? DifficultyVariant.Easy
      : DifficultyVariant.Medium;

    var state = new SettingState {DifficultyVariant = difficultyVariant};

    return new Dictionary<string, SettingState> {{"settings", state}};
  }
}