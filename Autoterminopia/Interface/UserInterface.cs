using Spectre.Console;
using Spectre.Console.Rendering;
using static Autoterminopia.Models.Enums;

namespace Autoterminopia.Interface
{
    internal class UserInterface
    {
        private readonly ConsoleUi _ui = new();
        public void ShowMainMenu()
        {
            AnsiConsole.Clear();
            PromptMainMenuWithDetails();

        }

        private MainMenuOptions PromptMainMenuWithDetails()
        {
            var options = Enum.GetValues<MainMenuOptions>().ToArray();
            int selected = 0;

            var layout = new Layout()
                .SplitColumns(
                    new Layout("menu").Ratio(2),
                    new Layout("details").Ratio(3)
                );

            string GetDetails(MainMenuOptions option) => option switch
            {
                MainMenuOptions.StartGame => "[bold]Start a new adventure[/]\nBegin your journey in Autoterminopia.",
                MainMenuOptions.Quit => "[bold]Quit[/]\nReturn to the mundane world (coward).",
                _ => "[grey]No description yet.[/]"
            };

            IRenderable BuildMenu()
            {
                var table = new Table()
                    .Border(TableBorder.None)
                    .HideHeaders()
                    .AddColumn("");

                for (int i = 0; i < options.Length; i++)
                {
                    var isSelected = i == selected;
                    var prefix = isSelected ? "[yellow]>[/] " : "  ";
                    var label = options[i].ToString();

                    // Highlight the selected row
                    var rowText = isSelected
                        ? $"{prefix}[black on yellow]{label}[/]"
                        : $"{prefix}{label}";

                    table.AddRow(rowText);
                }

                return new Panel(table)
                    .Border(BoxBorder.Rounded)
                    .Header(" Main Menu ")
                    .Padding(1, 0);
            }

            IRenderable BuildDetails()
                => new Panel(GetDetails(options[selected]))
                    .Border(BoxBorder.Rounded)
                    .Header(" Pixel Lord ")
                    .Padding(1, 1);

            layout["menu"].Update(BuildMenu());
            layout["details"].Update(BuildDetails());

            MainMenuOptions chosen = default;

            AnsiConsole.Live(layout).Start(ctx =>
            {
                ConsoleKey key;
                do
                {
                    // Render current state
                    layout["menu"].Update(BuildMenu());
                    layout["details"].Update(BuildDetails());
                    ctx.Refresh();

                    // Read input
                    key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.UpArrow)
                        selected = (selected - 1 + options.Length) % options.Length;
                    else if (key == ConsoleKey.DownArrow)
                        selected = (selected + 1) % options.Length;

                } while (key != ConsoleKey.Enter);

                chosen = options[selected];
            });

            return chosen;
        }
    }
}

