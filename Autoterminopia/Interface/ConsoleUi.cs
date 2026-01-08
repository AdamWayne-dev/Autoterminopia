using Spectre.Console;
using Spectre.Console.Rendering;


namespace Autoterminopia.Interface
{
    internal class ConsoleUi
    {
        public void DisplayMessage(string message, bool isError, string colour = "yellow")
        {

            var panel = new Panel(message)
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1),
                Header = new PanelHeader(isError ? "[bold red]Error[/]" : "[bold green]Info[/]")
            };
            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[grey] Press any key to continue...[/]");
            Console.ReadKey(true);
        }

        public void NarrativeSpeechBox(string speakerName, string message)
        {
            var panel = new Panel(message)
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1),
                Header = new PanelHeader($"[bold blue]{speakerName}[/]")
            };
            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
        }
    }
}
