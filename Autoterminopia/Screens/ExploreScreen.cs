using Autoterminopia.Interface;
using Autoterminopia.Game;
using static Autoterminopia.Models.Enums;

namespace Autoterminopia.Screens
{
    internal class ExploreScreen : IScreen
    {
        private readonly UserInterface _ui;
        private ExploreService exploreService = new ExploreService();
        public ExploreScreen(UserInterface ui) => _ui = ui;

        public IScreen Run(GameState state)
        {
            exploreService.Explore(); 
            return new AdventureMenuScreen(_ui);
        }
    }
}
