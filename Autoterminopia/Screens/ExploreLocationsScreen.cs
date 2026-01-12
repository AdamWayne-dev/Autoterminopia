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
        // Demo data (swap later for real models)
        var locations = new[]
        {
            new LocationChoice("Forest", new[] { "Path", "Hidden Grove", "Ruins Edge" }),
            new LocationChoice("Caves",  new[] { "Mouth", "Deep Tunnel", "Crystal Chamber" }),
        };

        // Selection state
        int locIndex = 0;
        int subIndex = 0;

        // Explore state
        bool exploring = false;
        bool paused = false;

        // Log
        var log = new List<string>
        {
            "Choose a location, press Enter to explore."
        };

        // Timer
        var stopwatch = Stopwatch.StartNew();
        var lastTickAt = stopwatch.Elapsed;
        TimeSpan tickInterval = TimeSpan.FromSeconds(2);

        // Styles (no markup needed)
        var selectedStyle = new Style(foreground: Color.Black, background: Color.Yellow);
        var hintStyle = new Style(foreground: Color.Grey);
        var dimStyle = new Style(foreground: Color.Grey);

        // Fixed left pane width = stable layout (tweak to taste)
        const int LeftWidth = 42;

        // ---------- Render builders ----------

        IRenderable BuildLeftPane()
        {
            var rows = new List<IRenderable>();

            // Title line (optional)
            rows.Add(new Text("Locations".PadRight(LeftWidth), new Style(decoration: Decoration.Bold)));
            rows.Add(new Text("".PadRight(LeftWidth)));

            for (int i = 0; i < locations.Length; i++)
            {
                bool locSelected = i == locIndex;

                // IMPORTANT: no extra prefix for selection; same width always
                string locLine = locations[i].Name.PadRight(LeftWidth);
                rows.Add(new Text(locLine, locSelected ? selectedStyle : Style.Plain));

                // Show sublocations only for selected location (like you had)
                if (locSelected)
                {
                    var subs = locations[i].Sublocations;
                    for (int s = 0; s < subs.Length; s++)
                    {
                        bool subSelected = s == subIndex;

                        // Indentation is constant (doesn't change on selection)
                        string subLine = ("  " + subs[s]).PadRight(LeftWidth);
                        rows.Add(new Text(subLine, subSelected ? selectedStyle : dimStyle));
                    }
                }

                rows.Add(new Text("".PadRight(LeftWidth)));
            }


            // Wrap the left content in a borderless container;
            // the outer table provides the frame and divider.
            return new Rows(rows);
        }

        IRenderable BuildRightPane()
        {
            var selected = locations[locIndex];
            var sub = selected.Sublocations[subIndex];

            var now = stopwatch.Elapsed;
            var remaining = tickInterval - (now - lastTickAt);
            if (remaining < TimeSpan.Zero) remaining = TimeSpan.Zero;

            string status = exploring
                ? (paused ? "Paused" : "Exploring")
                : "Idle";

            // Show last N lines
            const int maxLines = 14;
            var recent = log.Count <= maxLines ? log : log.Skip(log.Count - maxLines).ToList();

            var top = new Panel(new Rows(
                    new Text($"Status: {status}"),
                    new Text($"Area: {selected.Name}   Sub: {sub}"),
                    new Text($"Next tick: {remaining:mm\\:ss\\.ff}", dimStyle)
                ))
                .Border(BoxBorder.Rounded)
                .Header(" Exploration ")
                .Padding(1, 1);

            var bottom = new Panel(new Text(string.Join("\n", recent), dimStyle))
                .Border(BoxBorder.Rounded)
                .Header(" Log ")
                .Padding(1, 1);

            return new Rows(top, new Text(""), bottom);
        }

        Table BuildRoot()
        {
            var root = new Table()
                .Border(TableBorder.Rounded)
                .HideHeaders()
                .Expand(); // Fill the console width (prevents “table got smaller”)

            // Left column fixed width, right column auto takes the rest
            root.AddColumn(new TableColumn("").Width(LeftWidth).NoWrap());
            root.AddColumn(new TableColumn("")); // no width set => uses remaining space

            root.AddRow(BuildLeftPane(), BuildRightPane());

            return root;
        }

        void Render(LiveDisplayContext ctx)
        {
            ctx.UpdateTarget(BuildRoot());
            ctx.Refresh();
        }

        // ---------- Live loop ----------
        AnsiConsole.Live(BuildRoot()).Start(ctx =>
        {
            ctx.Refresh();

            while (true)
            {
                bool changed = false;

                // Handle all queued input this frame
                while (Console.KeyAvailable)
                {
                    changed = true;
                    var key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.Escape)
                        return;

                    // Navigation (only lock selection if you want; currently allow while idle only)
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
                        log.Add($"Started exploring: {locations[locIndex].Name} -> {locations[locIndex].Sublocations[subIndex]}");
                    }
                    else if (key == ConsoleKey.Spacebar && exploring)
                    {
                        paused = !paused;
                        log.Add(paused ? "Paused." : "Resumed.");
                    }
                }

                // Tick
                if (exploring && !paused)
                {
                    var now = stopwatch.Elapsed;
                    if (now - lastTickAt >= tickInterval)
                    {
                        changed = true;
                        lastTickAt = now;

                        // Placeholder; later replace with your service tick/result
                        log.Add("Tick... you find something interesting.");
                    }
                }

                // Render only when something changed (no jitter, no bell spam)
                if (changed)
                    Render(ctx);

                Thread.Sleep(33);
            }
        });

        return new AdventureMenuScreen(_ui, _exploreService);
    }

    private record LocationChoice(string Name, string[] Sublocations);
}
