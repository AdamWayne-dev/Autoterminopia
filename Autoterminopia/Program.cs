using Autoterminopia.Interface;
using Microsoft.Extensions.Configuration;
using Autoterminopia.Data;
using Autoterminopia.Screens;
using Autoterminopia.Game;

namespace Autoterminopia
{
    internal class Program
    {
        static void Main(string[] args)
        {
 
            var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

            var databasePath = configuration["Database:ConnectionString"];
            var databaseInitialiser = new DatabaseInitialiser(databasePath);
            var seedInitialData = new SeedInitialData(databasePath);

            databaseInitialiser.Initialise();
            seedInitialData.Seed();

            var ui = new UserInterface();
            var gameState = new GameState();
            var exploreService = new ExploreService();

            IScreen startScreen = new MainMenuScreen(ui, exploreService);

            var game = new GameController(gameState, startScreen);
            game.Run();
        }
    }
}