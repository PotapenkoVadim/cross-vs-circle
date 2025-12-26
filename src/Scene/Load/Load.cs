internal class Load : Scene
{
  private readonly GameLoader _loader = new();
  private readonly AppState _state;

  public Load(AppState state)
  {
    _state = state;
  }
  public override void HandleUserInput(InputKeys? userInput)
  {
    throw new NotImplementedException();
  }

  public override void Render()
  {
    Console.WriteLine("LOAD SCENE");
  }

  public override void Update()
  {
    throw new NotImplementedException();
  }
}