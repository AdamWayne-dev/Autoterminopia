using Autoterminopia.Interface;
using Autoterminopia.Game;
using static Autoterminopia.Models.Enums;

namespace Autoterminopia.Screens
{
    internal class ViewStatsScreen : IScreen
    {
        private readonly UserInterface _ui;

        public ViewStatsScreen(UserInterface ui) => _ui = ui;

        public IScreen Run(GameState state)
        {

            return new AdventureMenuScreen(_ui);
        }
    }
}
