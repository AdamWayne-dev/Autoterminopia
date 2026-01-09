using Autoterminopia.Interface;
using Autoterminopia.Game;
using static Autoterminopia.Models.Enums;

namespace Autoterminopia.Screens
{
    internal class ShopScreen : IScreen
    {
        private readonly UserInterface _ui;

        public ShopScreen(UserInterface ui) => _ui = ui;

        public IScreen Run(GameState state)
        {

            return new AdventureMenuScreen(_ui);
        }
    }
}
