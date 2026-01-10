using Spectre.Console;
using static Autoterminopia.Models.Enums;
namespace Autoterminopia.Game
{
    internal class ExploreService
    {
        public ExploreService() { }

        public void Explore()
        {
            bool isExploring = true;
            while (isExploring)
            {
                ExploreTimer(5f);// Explore for 10 seconds

                Random rand = new();
                List<EncounterList> encounters = new();
                encounters = Enum.GetValues<EncounterList>().ToList();

                var encounter = encounters[rand.Next(encounters.Count)];

                switch (encounter)
                {
                    case EncounterList.FindItem:
                        AnsiConsole.MarkupLine("[yellow]You found a mysterious item![/]");
                        break;
                    case EncounterList.BattleEnemy:
                        AnsiConsole.MarkupLine("[red]An enemy appears! Prepare for battle![/]");
                        break;
                    case EncounterList.DiscoverLocation:
                        AnsiConsole.MarkupLine("[blue]You discovered a new location![/]");
                        break;
                    case EncounterList.NothingHappens:
                        AnsiConsole.MarkupLine("[grey]Nothing happened during your exploration.[/]");
                        break;
                }

                isExploring = false;
                Console.ReadKey();
                // Exploration logic goes here
                // For example, random encounters, finding items, etc.
                // For demonstration, we'll just end the exploration after one loop

            }
        }

        public void ExploreTimer(float durationInSeconds)
        {
            AnsiConsole.Clear();
            var timer = 0f;
            var interval = 0.5f; // Update every half second
            while (timer < durationInSeconds)
            {
                AnsiConsole.MarkupLine($"[green]Exploring... {timer + interval}/{durationInSeconds} seconds[/]");
                System.Threading.Thread.Sleep((int)(interval * 1000)); // Sleep for the interval
                timer += interval;
                AnsiConsole.Clear();

            }
        }
    }
}
