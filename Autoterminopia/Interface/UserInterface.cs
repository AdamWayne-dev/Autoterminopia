using Spectre.Console;
using Spectre.Console.Rendering;
using static Autoterminopia.Models.Enums;

namespace Autoterminopia.Interface
{
    internal class UserInterface
    {
        public MainMenuOptions PromptMainMenu()
        {
            AnsiConsole.Clear();

            var options = Enum.GetValues<MainMenuOptions>().ToArray();

            return PromptMenu(
                options,
                option => option switch
                {
                    MainMenuOptions.StartGame => "Start Game",
                    MainMenuOptions.Quit => "Quit",
                    _ => option.ToString()
                },
                option => option switch
                {
                    MainMenuOptions.StartGame =>
                        "[bold]Start a new adventure[/]\nBegin your journey in Autoterminopia.",
                    MainMenuOptions.Quit =>
                        "[bold]Quit[/]\nReturn to the mundane world (coward).",
                    _ => "[grey]...[/]"
                },
                header: "Main Menu",
                detailsHeader: "Pixel Lord"
            );
        }

        public void PromptAdventureMenu()
        {
            AnsiConsole.Clear();
            var options = Enum.GetValues<AdventureMenuOptions>().ToArray();
            var choice = PromptMenu(
                options,
                option => option switch
                {
                    AdventureMenuOptions.Explore => "Explore",
                    AdventureMenuOptions.ViewStats => "View Stats",
                    AdventureMenuOptions.ViewInventory => "View Inventory",
                    AdventureMenuOptions.Shop => "Shop",
                    AdventureMenuOptions.ExitToMainMenu => "Exit to Main Menu",
                    _ => option.ToString()
                },
                option => option switch
                {
                    AdventureMenuOptions.Explore => "[bold]Explore the world[/]\nVenture into unknown territories and face challenges.",
                    AdventureMenuOptions.ViewStats => "[bold]View your stats[/]\nCheck your character's attributes and progress.",
                    AdventureMenuOptions.ViewInventory => "[bold]View your inventory[/]\nSee the items you have collected on your journey.",
                    AdventureMenuOptions.Shop => "[bold]Visit the shop[/]\nBuy and sell items to aid you in your adventure.",
                    AdventureMenuOptions.ExitToMainMenu => "[bold]Exit to Main Menu[/]\nReturn to the main menu to start a new game or quit.",
                    _ => "[grey]...[/]"
                },
                header: "Adventure Menu",
                detailsHeader: "Choose Your Action"
            );

            switch (choice)
            {
                case AdventureMenuOptions.Explore:
                    Explore();
                    break;
                case AdventureMenuOptions.ViewStats:
                    ViewStats();
                    break;
                case AdventureMenuOptions.ViewInventory:
                    ViewInventory();
                    break;
                case AdventureMenuOptions.Shop:
                    Shop();
                    break;
                case AdventureMenuOptions.ExitToMainMenu:
                    PromptMainMenu();
                    break;
            }
        }

        public T PromptMenu<T>(
                IReadOnlyList<T> options, Func<T, string> label, Func<T, string> description,
                string header = "Menu",
                string detailsHeader = "Info"
        )
        {
            int selected = 0;

            var layout = new Layout()
                .SplitColumns(
                    new Layout("menu").Ratio(2),
                    new Layout("details").Ratio(3)
                );

            IRenderable BuildMenu()
            {
                var table = new Table()
                    .Border(TableBorder.None)
                    .HideHeaders()
                    .AddColumn("");

                for (int i = 0; i < options.Count; i++)
                {
                    var isSelected = i == selected;
                    var prefix = isSelected ? "[yellow]>[/] " : "  ";
                    var text = label(options[i]);

                    var rowText = isSelected
                        ? $"{prefix}[black on yellow]{text}[/]"
                        : $"{prefix}{text}";

                    table.AddRow(rowText);
                }

                return new Panel(table)
                    .Border(BoxBorder.Rounded)
                    .Header($" {header} ")
                    .Padding(1, 0);
            }

            IRenderable BuildDetails()
                => new Panel(description(options[selected]))
                    .Border(BoxBorder.Rounded)
                    .Header($" {detailsHeader} ")
                    .Padding(1, 1);

            layout["menu"].Update(BuildMenu());
            layout["details"].Update(BuildDetails());

            var chosen = default(T)!;

            AnsiConsole.Live(layout).Start(ctx =>
            {
                ConsoleKey key;
                do
                {
                    layout["menu"].Update(BuildMenu());
                    layout["details"].Update(BuildDetails());
                    ctx.Refresh();

                    key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.UpArrow)
                        selected = (selected - 1 + options.Count) % options.Count;
                    else if (key == ConsoleKey.DownArrow)
                        selected = (selected + 1) % options.Count;

                } while (key != ConsoleKey.Enter);

                chosen = options[selected];
            });

            return chosen;
        }


    }
}

