using Spectre.Console;
using CodingTracker.Models;
using CodingTracker.Services;

internal class UserInterface
{
    private readonly SessionService _service;

    public UserInterface(SessionService service)
    {
        _service = service;
    }

    public void MainMenu()
    {
        bool running = true;

        while (running)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<MenuOption>()
                    .Title("[yellow]Please select a choice[/]")
                    .AddChoices(Enum.GetValues<MenuOption>())
            );

            switch (choice)
            {
                case MenuOption.GenerateReport:
                    ShowByMonth("report");
                    break;

                case MenuOption.ViewSessions:
                    ShowByMonth("show");
                    break;

                case MenuOption.AddSession:
                    ChooseInsert();
                    break;

                case MenuOption.UpdateSession:
                    ShowByMonth("update");
                    break;

                case MenuOption.DeleteSession:
                    ShowByMonth("delete");
                    break;

                case MenuOption.Exit:
                    running = false;
                    return;
            }
        }
    }

    private void ShowByMonth(string action)
    {
        int year, month;
        var sessions = GetSessionsForYearMonth(out year, out month);

        if (!sessions.Any())
        {
            AnsiConsole.MarkupLine("[red]No sessions found for this month/year.[/]");
            Console.ReadKey(true);
            return;
        }

        Console.Clear();

        switch (action)
        {
            case "show":
                BuildTable(sessions, false);
                break;

            case "report":
                BuildTable(sessions, true);
                break;

            case "update":
                BuildTable(sessions, false);
                UpdateSessions(sessions);
                break;

            case "delete":
                BuildTable(sessions, false);
                DeleteSessions(sessions);
                break;

            default:
                AnsiConsole.MarkupLine("[red]Unknown action[/]");
                break;
        }

        AnsiConsole.MarkupLine("\n[grey]Press any key to return...[/]");
        Console.ReadKey(true);
    }

    private List<CodingSession> GetSessionsForYearMonth(out int year, out int month)
    {
        year = 0;
        month = 0;

        var years = _service.GetAvailableYears();
        if (!years.Any()) return new List<CodingSession>();

        year = PromptForYear(years);

        var months = _service.GetAvailableMonths(year);
        if (!months.Any()) return new List<CodingSession>();

        month = PromptForMonth(months, year);

        return _service.GetSessionsByMonth(year, month);
    }

    private void UpdateSessions(List<CodingSession> sessions)
    {
        int id = AnsiConsole.Prompt(
            new SelectionPrompt<int>()
                .Title("[yellow]Select the ID of the session to update[/]")
                .AddChoices(sessions.Select(s => s.Id))
        );

        DateTime newStart = PromptForDateTime("Enter new start time (dd-MM-yy HH:mm):");
        DateTime newEnd;
        
       
        while (true)
        {
            
            newEnd = PromptForDateTime("Enter new end time (dd-MM-yy HH:mm):");
            
            if(!_service.ValidateTimes(newStart,newEnd)) 
            AnsiConsole.MarkupLine("[yellow]/n End time cannot be before start time. Try again.[/]");
            else
            {
                break;
            }
        }

        if(_service.UpdateSession(id, newStart, newEnd))
        {
            AnsiConsole.MarkupLine($"[yellow]Session {id} updated successfully![/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[yellow]Error ocurred[/]");
        }
    }

    private void DeleteSessions(List<CodingSession> sessions)
    {
        int id = AnsiConsole.Prompt(
            new SelectionPrompt<int>()
                .Title("[yellow]Select the ID of the session to delete[/]")
                .AddChoices(sessions.Select(s => s.Id))
        );

        
        bool result = _service.DeleteSession(id);
        if(result)AnsiConsole.MarkupLine($"[yellow]Session {id} deleted successfully![/]");
        else
        {
            AnsiConsole.MarkupLine($"[yellow]Session {id} deleted successfully![/]");
        }
    }       




    private void BuildTable(List<CodingSession> sessions, bool includeTotal = false)
    {
        Console.Clear();

        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Start Date");
        table.AddColumn("End Date");
        table.AddColumn("Start Time");
        table.AddColumn("End Time");
        table.AddColumn("Duration");

        foreach (var s in sessions)
        {
            table.AddRow(
                s.Id.ToString(),
                s.StartTime.ToString("dd-MM-yy"),
                s.EndTime.ToString("dd-MM-yy"),
                s.StartTime.ToString("HH:mm:ss"),
                s.EndTime.ToString("HH:mm:ss"),
                s.GetDurationAsString()
            );
        }

        if (includeTotal)
        {
            int totalSeconds = sessions.Sum(s => s.Duration);
            TimeSpan total = TimeSpan.FromSeconds(totalSeconds);

            table.AddRow(
                "[yellow]TOTAL[/]",
                "",
                "",
                "",
                "",
                $"[yellow]{total:hh\\:mm\\:ss}[/]"
            );
        }

        AnsiConsole.Write(table);
    }

    private void ChooseInsert()
    {
        Console.Clear();
        var choice = PromptForInsert();

        DateTime startTime;
        DateTime endTime;

        if (choice == "Stopwatch")
        {
            startTime = DateTime.Now;
            AnsiConsole.MarkupLine("[green]Stopwatch started. Press any key to stop...[/]");

            while (!Console.KeyAvailable)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write($"Elapsed: {(DateTime.Now - startTime):hh\\:mm\\:ss}");
                Thread.Sleep(200);
            }

            Console.ReadKey(true);
            endTime = DateTime.Now;
        }
        else
        {
            startTime = PromptForDateTime("Enter start time (dd-MM-yy HH:mm):");

            while (true)
            {
                endTime = PromptForDateTime("Enter end time (dd-MM-yy HH:mm):");

                if (_service.ValidateTimes(startTime, endTime)) break;

                AnsiConsole.MarkupLine("[red]End time cannot be before start time.[/]");
            }
        }

        _service.AddSession(new CodingSession(0, startTime, endTime));

        AnsiConsole.MarkupLine("[green]\nSession added successfully![/]");
    }

    private DateTime PromptForDateTime(string prompt)
    {
        while (true)
        {
            var input = AnsiConsole.Ask<string>(prompt);

            if (DateTime.TryParseExact(
                    input,
                    "dd-MM-yy HH:mm",
                    null,
                    System.Globalization.DateTimeStyles.None,
                    out var dt))
            {
                return dt;
            }

            AnsiConsole.MarkupLine("[red]Invalid format. Use dd-MM-yy HH:mm[/]");
        }
    }

    private String PromptForInsert()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Choose insert method[/]")
                .AddChoices(new[] { "Stopwatch", "Insert manually" })
        );
    }
    private int PromptForYear(List<int> years)
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<int>()
                .Title("[yellow]Select a year[/]")
                .AddChoices(years)
        );
    }

    private int PromptForMonth(List<int> months, int year)
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<int>()
                .Title($"[yellow]Select a month in {year}[/]")
                .AddChoices(months)
        );
    }
}
