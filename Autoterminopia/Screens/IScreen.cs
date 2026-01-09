using Autoterminopia.Game;

namespace Autoterminopia.Screens
{
    internal interface IScreen
    {
        IScreen Run(GameState state);
    }
}
