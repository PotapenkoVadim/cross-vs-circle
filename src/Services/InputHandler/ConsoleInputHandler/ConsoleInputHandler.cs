internal class ConsoleInputHandler : IUserInputHandler
{
  public InputKeys? InputHandler()
  {
    if (Console.KeyAvailable)
    {
      ConsoleKeyInfo userInput = Console.ReadKey(true);
      return userInput.Key switch
      {
        ConsoleKey.UpArrow => InputKeys.Up,
        ConsoleKey.DownArrow => InputKeys.Down,
        ConsoleKey.LeftArrow => InputKeys.Left,
        ConsoleKey.RightArrow => InputKeys.Right,

        ConsoleKey.W => InputKeys.Up,
        ConsoleKey.S => InputKeys.Down,
        ConsoleKey.A => InputKeys.Left,
        ConsoleKey.D => InputKeys.Right,

        ConsoleKey.Enter => InputKeys.Accept,
        ConsoleKey.Y => InputKeys.Accept,

        ConsoleKey.Escape => InputKeys.Decline,
        ConsoleKey.Backspace => InputKeys.Decline,
        ConsoleKey.N => InputKeys.Decline,

        ConsoleKey.F5 => InputKeys.QuickSave,

        _ => null
      };
    }

    return null;
  }
}