using Autoterminopia.Game;
using Autoterminopia.Interface;
using Autoterminopia.Screens;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Diagnostics;

internal class ExploreLocationsScreen : IScreen
{
    private readonly UserInterface _ui;
    private readonly ExploreService _explore; // your mechanics class

    public ExploreLocationsScreen(UserInterface ui, ExploreService explore)
    {
        _ui = ui;
        _explore = explore;
    }

    public IScreen Run(GameState state)
    {
        // Replace these with your real area/location models later
        var locations = new[]
        {
            new LocationChoice("Forest", new[] { "Path", "Hidden Grove", "Ruins Edge" }),
            new LocationChoice("Caves",  new[] { "Mouth", "Deep Tunnel", "Crystal Chamber" }),
        };

        int locIndex = 0;
        int subIndex = 0;

        bool exploring = false;
        bool paused = false;

        var log = new List<string>();
        log.Add("[grey]Choose a location, press Enter to explore.[/]");

        var stopwatch = Stopwatch.StartNew();
        var lastTickAt = stopwatch.Elapsed;

        TimeSpan tickInterval = TimeSpan.FromSeconds(2); // later: mastery can affect this

        var layout = new Layout()
            .SplitColumns(
                new Layout("left").Ratio(2),
                new Layout("right").Ratio(3)
            );

        IRenderable BuildLeft()
        {
            var table = new Table().Border(TableBorder.None).HideHeaders();
            table.AddColumn("");

            for (int i = 0; i < locations.Length; i++)
            {
                var isSelectedLoc = i == locIndex;
                var prefix = isSelectedLoc ? "[yellow]>[/] " : "  ";
                var label = isSelectedLoc
                    ? $"{prefix}[black on yellow]{locations[i].Name}[/]"
                    : $"{prefix}{locations[i].Name}";

                table.AddRow(label);

                // show sublocations for selected location
                if (isSelectedLoc)
                {
                    var subs = locations[i].Sublocations;
                    for (int s = 0; s < subs.Length; s++)
                    {
                        var isSelectedSub = s == subIndex;
                        var subPrefix = isSelectedSub ? "    [yellow]•[/] " : "      ";
                        var subLabel = isSelectedSub
                            ? $"{subPrefix}[black on yellow]{subs[s]}[/]"
                            : $"{subPrefix}{subs[s]}";
                        table.AddRow(subLabel);
                    }
                }
            }

            var hint = "[grey]↑/↓ location  ←/→ sub  Enter explore  Space pause  Esc back[/]";
            return new Panel(new Rows(table, new Markup(hint)))
                .Border(BoxBorder.Rounded)
                .Header(" Locations ")
                .Padding(1, 0);
        }

        IRenderable BuildRight()
        {
            var selected = locations[locIndex];
            var sub = selected.Sublocations[subIndex];

            var now = stopwatch.Elapsed;
            var elapsedSinceTick = now - lastTickAt;
            var remaining = tickInterval - elapsedSinceTick;
            if (remaining < TimeSpan.Zero) remaining = TimeSpan.Zero;

            var status = exploring
                ? (paused ? "[yellow]Paused[/]" : "[green]Exploring[/]")
                : "[grey]Idle[/]";

            // show last ~12 log lines
            var recent = log.Count <= 12 ? log : log.Skip(log.Count - 12).ToList();
            var logPanel = new Panel(new Markup(string.Join("\n", recent)))
                .Border(BoxBorder.Rounded)
                .Header(" Log ")
                .Padding(1, 1);

            var header = new Markup(
                $"Status: {status}\n" +
                $"Area: [bold]{selected.Name}[/]  Sub: [bold]{sub}[/]\n" +
                $"Next tick: [grey]{remaining:mm\\:ss\\.ff}[/]"
            );

            return new Panel(new Rows(header, new Rule(), logPanel))
                .Border(BoxBorder.Rounded)
                .Header(" Exploration ")
                .Padding(1, 1);
        }

        layout["left"].Update(BuildLeft());
        layout["right"].Update(BuildRight());

        AnsiConsole.Live(layout).Start(ctx =>
        {
            while (true)
            {
                // 1) Input (non-blocking)
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.Escape)
                        break;

                    if (!exploring) // only allow changing selection while not exploring (your choice)
                    {
                        if (key == ConsoleKey.UpArrow)
                        {
                            locIndex = (locIndex - 1 + locations.Length) % locations.Length;
                            subIndex = 0;
                        }
                        else if (key == ConsoleKey.DownArrow)
                        {
                            locIndex = (locIndex + 1) % locations.Length;
                            subIndex = 0;
                        }
                        else if (key == ConsoleKey.LeftArrow)
                        {
                            var subs = locations[locIndex].Sublocations;
                            subIndex = (subIndex - 1 + subs.Length) % subs.Length;
                        }
                        else if (key == ConsoleKey.RightArrow)
                        {
                            var subs = locations[locIndex].Sublocations;
                            subIndex = (subIndex + 1) % subs.Length;
                        }
                    }

                    if (key == ConsoleKey.Enter)
                    {
                        exploring = true;
                        paused = false;
                        lastTickAt = stopwatch.Elapsed;
                        log.Add($"[green]Started exploring[/] {locations[locIndex].Name} → {locations[locIndex].Sublocations[subIndex]}");
                    }
                    else if (key == ConsoleKey.Spacebar && exploring)
                    {
                        paused = !paused;
                        log.Add(paused ? "[yellow]Paused.[/]" : "[green]Resumed.[/]");
                    }
                }

                // 2) Tick
                if (exploring && !paused)
                {
                    var now = stopwatch.Elapsed;
                    if (now - lastTickAt >= tickInterval)
                    {
                        lastTickAt = now;

                        // Call your actual mechanic here:
                        // var result = _explore.Tick(state, locations[locIndex], subIndex);
                        // log.AddRange(result.Lines);

                        // Placeholder:
                        log.Add("[grey]Tick...[/] You find something interesting.");
                    }
                }

                // 3) Render
                layout["left"].Update(BuildLeft());
                layout["right"].Update(BuildRight());
                ctx.Refresh();

                // 4) Throttle loop so it doesn’t melt your CPU
                Thread.Sleep(33);
            }
        });

        // Back to whatever screen makes sense
        return new AdventureMenuScreen(_ui, _explore);
    }

    private record LocationChoice(string Name, string[] Sublocations);
}
