using Autoterminopia.Game;
using Autoterminopia.Interface;
using static Autoterminopia.Models.Enums;

namespace Autoterminopia.Screens
{
    internal class MainMenuScreen : IScreen
    {
        private readonly UserInterface _ui;
        private readonly ExploreService _exploreService;

        public MainMenuScreen(UserInterface ui, ExploreService ex)
        {
            _ui = ui; 
            _exploreService = ex;
        }

        public IScreen Run(GameState state)
        {
            var choice = _ui.PromptMainMenu();

            return choice switch
            {
                MainMenuOptions.StartGame => new AdventureMenuScreen(_ui, _exploreService),
                MainMenuOptions.Options => new OptionsScreen(_ui),
                MainMenuOptions.Quit => new ExitScreen(_ui),
                _ => this
            };
        }
    }
}
