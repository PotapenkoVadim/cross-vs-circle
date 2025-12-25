internal class FileManager {
  private string _filePath;
  public FileManager(string filePath) {
    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);

    if (!File.Exists(fullPath))
      File.Create(fullPath).Close();

    _filePath = fullPath;
  }

  public string ReadFile() {
    return File.ReadAllText(_filePath);
  }

  public void WriteFile(string content) {
    File.AppendAllText(_filePath, content);
  }
}