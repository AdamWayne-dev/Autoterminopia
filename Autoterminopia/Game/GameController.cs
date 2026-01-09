using Autoterminopia.Game;
using Autoterminopia.Interface;
using Autoterminopia.Screens;

namespace Autoterminopia.Game
{
    internal class GameController
    {
        private readonly UserInterface _ui;
        private IScreen _currentScreen;
        private readonly GameState _gameState;
        public GameController(UserInterface ui)
        {
            _ui = ui;
            _currentScreen = new MainMenuScreen(_ui);
            _gameState = new GameState();
        }
        public void StartGameLoop()
        {
            while (true)
            {
                _currentScreen = _currentScreen.Run(_gameState);
            }
        }
    }
}
