using Autoterminopia.Interface;
using Autoterminopia.Game;
using static Autoterminopia.Models.Enums;

namespace Autoterminopia.Screens
{
    internal class AdventureMenuScreen: IScreen
    {
        private readonly UserInterface _ui;

        public AdventureMenuScreen(UserInterface ui) => _ui = ui;

        public IScreen Run(GameState state)
        {
            var choice = _ui.PromptAdventureMenu();
            
            return choice switch
            {
                AdventureMenuOptions.Explore => new ExploreScreen(_ui),
                AdventureMenuOptions.ViewStats => new ViewStatsScreen(_ui),
                AdventureMenuOptions.ViewInventory => new ViewInventoryScreen(_ui),
                AdventureMenuOptions.Shop => new ShopScreen(_ui),
                AdventureMenuOptions.ExitToMainMenu => new MainMenuScreen(_ui),
                _ => this
            };
        }
    }
}
