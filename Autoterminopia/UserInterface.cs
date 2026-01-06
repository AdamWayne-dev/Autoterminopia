using Spectre.Console;
using static Autoterminopia.Models.Enums;

namespace Autoterminopia
{
    internal class UserInterface
    {
        public void ShowMainMenu()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Welcome to Autoterminopia![/]");
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<MainMenuOptions>()
                    .Title("Please select an option:")
                    .AddChoices(Enum.GetValues<MainMenuOptions>()));
        }
    }
}

