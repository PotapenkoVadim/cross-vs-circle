internal abstract class Scene
{
  public abstract void Render();
  public abstract void Update();
  public abstract void HandleUserInput(InputKeys? userInput);
}