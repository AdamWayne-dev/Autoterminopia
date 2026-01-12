using Autoterminopia.Interface;
using Autoterminopia.Game;
using static Autoterminopia.Models.Enums;

namespace Autoterminopia.Screens
{
    internal class ExploreScreen : IScreen
    {
        private readonly UserInterface _ui;
        private readonly ExploreService _exploreService;
        public ExploreScreen(UserInterface ui, ExploreService ex)
        {
            _ui = ui;
            _exploreService = ex;
        }

        public IScreen Run(GameState state)
        { 
            return new ExploreLocationsScreen(_ui, _exploreService);
        }
    }
}
