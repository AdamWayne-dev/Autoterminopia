namespace Autoterminopia.Models
{
    internal class Enums
    {
        public enum MainMenuOptions
        {
            StartGame = 1,
            Options = 2,
            Quit = 3
        }

        public enum OptionsMenuOptions
        {
            ResetAllData = 1,
            ReturnToMainMenu = 2
        }
        public enum AdventureMenuOptions
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
