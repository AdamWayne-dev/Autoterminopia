namespace Autoterminopia.Models
{
    internal class Enums
    {
        public enum MainMenuOptions
        {
            StartGame = 1,
            Quit = 2
        }

        public enum InGameMenuOptions
        {
            Explore = 1,
            ViewStats = 2,
            ViewInventory = 3,
            Shop = 4,
            ExitToMainMenu = 5
        }

        public enum ExploreMenu 
        { 
            ChooseLevel = 1,
            ReturnToTown = 2
        }
    }
}
