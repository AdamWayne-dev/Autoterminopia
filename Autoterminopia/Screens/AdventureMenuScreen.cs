using Autoterminopia.Interface;
using Autoterminopia.Game;

namespace Autoterminopia.Screens
{
    internal class AdventureMenuScreen: IScreen
    {
        private readonly UserInterface _ui;

        public AdventureMenuScreen(UserInterface ui) => _ui = ui;

        public IScreen Run(GameState state)
        {
            _ui.PromptAdventureMenu();
            return this;
        }
    }
}
