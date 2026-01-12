using Autoterminopia.Interface;
using Autoterminopia.Game;
using static Autoterminopia.Models.Enums;

namespace Autoterminopia.Screens
{
    internal class AdventureMenuScreen: IScreen
    {
        private readonly UserInterface _ui;
        private readonly ExploreService _explore;

        public AdventureMenuScreen(UserInterface ui, ExploreService ex)
        {
            _ui = ui; 
            _explore = ex;
        }

        public IScreen Run(GameState state)
        {
            var choice = _ui.PromptAdventureMenu();
            
            return choice switch
            {
                AdventureMenuOptions.Explore => new ExploreScreen(_ui, _explore),
                AdventureMenuOptions.ViewStats => new ViewStatsScreen(_ui),
                AdventureMenuOptions.ViewInventory => new ViewInventoryScreen(_ui),
                AdventureMenuOptions.Shop => new ShopScreen(_ui),
                AdventureMenuOptions.ExitToMainMenu => new MainMenuScreen(_ui, _explore),
                _ => this
            };
        }
    }
}
