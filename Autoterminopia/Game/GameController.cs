using Autoterminopia.Game;
using Autoterminopia.Screens;

internal class GameController
{
    private readonly GameState _state;
    private IScreen _current;

    public GameController(GameState state, IScreen startScreen)
    {
        _state = state;
        _current = startScreen;
    }

    public void Run()
    {
        while (_current is not ExitScreen)
        {
            _current = _current.Run(_state);
        }
    }
}