internal class ErrorLogger {
  private FileManager _fileManager;
  public ErrorLogger() {
    _fileManager = new FileManager("error.log");
  }

  public void LogError(string error) {
    try {
      string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      string logMessage = $"{timestamp} - {error}";
      _fileManager.WriteFile(logMessage + "\n");
    } catch {
      throw new Exception("Failed to log error");
    }
  }
}