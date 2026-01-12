using Autoterminopia.Interface;
using Autoterminopia.Game;
using static Autoterminopia.Models.Enums;

namespace Autoterminopia.Screens
{
    internal class OptionsScreen : IScreen
    {
        private readonly UserInterface _ui;
        private readonly ExploreService _exploreService;


        public OptionsScreen(UserInterface ui) => _ui = ui;

        public IScreen Run(GameState state)
        {

            var choice = _ui.PromptOptionsMenu();

            return choice switch
            {
                OptionsMenuOptions.ResetAllData => new AdventureMenuScreen(_ui, _exploreService),
                OptionsMenuOptions.ReturnToMainMenu => new MainMenuScreen(_ui, _exploreService),
                _ => this
            };
        }
    }
}
