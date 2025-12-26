internal class GameLoader
{
  private const string DB_FILE = "database.db";
  private const int TARGET_VERSION = 1;
  private readonly DataBaseManager _dbManager;
  public GameLoader()
  {
    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), DB_FILE);
    _dbManager = new DataBaseManager(fullPath);

    ApplyMigrations();
  }

  public void Save(GameState state) {}

  public void Load() {}

  private void ApplyMigrations()
  {
    int currentVersion = _dbManager.GetVersion();

     if (currentVersion > TARGET_VERSION) throw new Exception($"Migration failed: current version is newer target");
    if (currentVersion == TARGET_VERSION) return;

    if (currentVersion < 1)
    {
      // MIGRATION: Create GameState table
      _dbManager.ExecuteNonQuery(@"
      CREATE TABLE IF NOT EXISTS GameState (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        created_at TEXT DEFAULT CURRENT_TIMESTAMP,
        board BLOB NOT NULL,
        player_x INTEGER NOT NULL CHECK(player_x > 0 AND player_x <= 10),
        player_y INTEGER NOT NULL CHECK(player_y > 0 AND player_y <= 10),
        ai_x INTEGER NOT NULL CHECK(ai_x > 0 AND ai_x <= 10),
        ai_y INTEGER NOT NULL CHECK(ai_y > 0 AND ai_y <= 10),
        turn INTEGER NOT NULL CHECK (turn IN (1, 0)),
        player_score INTEGER NOT NULL CHECK(player_score > 0),
        ai_score INTEGER NOT NULL CHECK(ai_score > 0)
      );
      ");
      _dbManager.SetVersion(1);
      currentVersion = 1;
    }

    if (currentVersion != TARGET_VERSION)
      throw new Exception($"Migration failed: current version {currentVersion}");
  }
}
