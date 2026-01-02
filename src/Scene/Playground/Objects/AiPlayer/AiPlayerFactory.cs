internal class AiPlayerFactory(DifficultyVariant difficulty) : IAiPlayer
{
  private readonly IAiPlayer _player = difficulty == DifficultyVariant.Easy
    ? new EasyAiPlayer()
    : new MediumAiPlayer();

  public void MakeMove(GameState state) => _player.MakeMove(state);
}