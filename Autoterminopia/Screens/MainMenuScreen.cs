using Autoterminopia.Game;
using Autoterminopia.Interface;
using static Autoterminopia.Models.Enums;

namespace Autoterminopia.Screens
{
    internal class MainMenuScreen : IScreen
    {
        private readonly UserInterface _ui;

        public MainMenuScreen(UserInterface ui) => _ui = ui;

        public IScreen Run(GameState state)
        {
            var choice = _ui.PromptMainMenu();

            return choice switch
            {
                MainMenuOptions.StartGame => new AdventureMenuScreen(_ui),
                MainMenuOptions.Quit => new ExitScreen(),
                _ => this
            };
        }
    }
}
