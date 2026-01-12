using Autoterminopia.Game;
using Autoterminopia.Interface;
using Autoterminopia.Screens;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Diagnostics;

internal class ExploreLocationsScreen : IScreen
{
    private readonly UserInterface _ui;
    private readonly ExploreService _exploreService;

    public ExploreLocationsScreen(UserInterface ui, ExploreService exploreService)
    {
        _ui = ui ?? throw new ArgumentNullException(nameof(ui));
        _exploreService = exploreService ?? throw new ArgumentNullException(nameof(exploreService));
    }

    public IScreen Run(GameState state)
    {
        var locations = new[]
        {
            new LocationChoice("Forest", new[] { "Path", "Hidden Grove", "Ruins Edge" }),
            new LocationChoice("Caves",  new[] { "Mouth", "Deep Tunnel", "Crystal Chamber" }),
        };


        int locIndex = 0;
        int subIndex = 0;

        bool exploring = false;
        bool paused = false;

        var log = new List<string>
        {
            "[grey]Choose a location, press Enter to explore.[/]"
        };

        var stopwatch = Stopwatch.StartNew();
        var lastTickAt = stopwatch.Elapsed;

        TimeSpan tickInterval = TimeSpan.FromSeconds(2);

        // ---------- BUILDERS ----------

        IRenderable BuildLeftContent()
        {
            var table = new Table()
                .Border(TableBorder.None)
                .HideHeaders()
                .AddColumn("");

            for (int i = 0; i < locations.Length; i++)
            {
                bool isSelectedLoc = i == locIndex;
                string locPrefix = isSelectedLoc ? "[yellow]>[/] " : "  ";

                string locRow = isSelectedLoc
                    ? $"{locPrefix}[black on yellow]{locations[i].Name}[/]"
                    : $"{locPrefix}{locations[i].Name}";

                table.AddRow(locRow);

                if (isSelectedLoc)
                {
                    var subs = locations[i].Sublocations;
                    for (int s = 0; s < subs.Length; s++)
                    {
                        bool isSelectedSub = s == subIndex;
                        string subPrefix = isSelectedSub ? "    [yellow]•[/] " : "      ";

                        string subRow = isSelectedSub
                            ? $"{subPrefix}[black on yellow]{subs[s]}[/]"
                            : $"{subPrefix}{subs[s]}";

                        table.AddRow(subRow);
                    }
                }
            }

            var hint = new Markup("[grey]↑/↓ location  ←/→ sub  Enter explore  Space pause  Esc back[/]");
            return new Rows(table, hint);
        }

        IRenderable BuildExplorationPanel()
        {
            var selected = locations[locIndex];
            var sub = selected.Sublocations[subIndex];

            var now = stopwatch.Elapsed;
            var remaining = tickInterval - (now - lastTickAt);
            if (remaining < TimeSpan.Zero) remaining = TimeSpan.Zero;

            var status = exploring
                ? (paused ? "[yellow]Paused[/]" : "[green]Exploring[/]")
                : "[grey]Idle[/]";

            var text = new Markup(
                $"Status: {status}\n" +
                $"Area: [bold]{selected.Name}[/]   Sub: [bold]{sub}[/]\n" +
                $"Next tick: [grey]{remaining:mm\\:ss\\.ff}[/]"
            );

            // Only this panel has a border (clean)
            return new Panel(text)
                .Border(BoxBorder.Rounded)
                .Header(" Exploration ")
                .Padding(1, 1)
                .Expand();
        }

        IRenderable BuildLogPanel()
        {
            const int maxLines = 14;
            var recent = log.Count <= maxLines ? log : log.Skip(log.Count - maxLines).ToList();

            return new Panel(new Markup(string.Join("\n", recent)))
                .Border(BoxBorder.Rounded)
                .Header(" Log ")
                .Padding(1, 1)
                .Expand();
        }

        IRenderable BuildRightContent()
        {
            // Stack the two right panels with a little spacing
            return new Rows(
                BuildExplorationPanel(),
                new Text(""), // spacer line
                BuildLogPanel()
            );
        }

        Table BuildRoot()
        {
            // Outer frame + vertical divider handled by the table border
            var root = new Table()
            .Border(TableBorder.Rounded)
            .HideHeaders()
            .AddColumn(new TableColumn(new Markup("[bold]Locations[/]")).Width(35).NoWrap())
            .AddColumn(new TableColumn(new Markup("[bold]Details[/]")));


            // Important: no borders on the left side content; table is the frame.
            root.AddRow(
                BuildLeftContent(),
                BuildRightContent()
            );

            return root;
        }

        void Render(LiveDisplayContext ctx)
        {
            ctx.UpdateTarget(BuildRoot());
            ctx.Refresh();
        }

        // ---------- LIVE LOOP ----------
        AnsiConsole.Live(BuildRoot()).Start(ctx =>
        {
            // initial paint
            ctx.Refresh();

            while (true)
            {
                bool changed = false;

                while (Console.KeyAvailable)
                {
                    changed = true;
                    var key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.Escape)
                        return;

                    if (!exploring)
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

                if (exploring && !paused)
                {
                    var now = stopwatch.Elapsed;
                    if (now - lastTickAt >= tickInterval)
                    {
                        changed = true;
                        lastTickAt = now;

                        // Placeholder tick result
                        log.Add("[grey]Tick...[/] You find something interesting.");
                    }
                }

                if (changed)
                    Render(ctx);

                Thread.Sleep(33);
            }
        });

        return new AdventureMenuScreen(_ui, _exploreService);
    }

    private record LocationChoice(string Name, string[] Sublocations);
}
