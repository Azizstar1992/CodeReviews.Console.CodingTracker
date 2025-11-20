using Spectre.Console;

using CodingTracker.Models;
using CodingTracker.Services;

internal class SessionMenuUI
{
    private readonly SessionService _service;

    public SessionMenuUI(SessionService service)
    {
        _service = service;
    }

    public void ShowByMonth(string action)
    {
        // 1. Traverse year/month and get sessions
        int year, month;
        var sessions = GetSessionsForYearMonth(out year, out month);

        if (!sessions.Any())
        {
            AnsiConsole.MarkupLine("[red]No sessions found for this month/year.[/]");
            Console.ReadKey(true);
            return;
        }

        Console.Clear();



        // 3. Branch by action
        switch (action.ToLower())
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
                DeleteSessions(sessions);
                break;

            default:
                AnsiConsole.MarkupLine("[red]Unknown action[/]");
                break;
        }



        // 5. Wait for key to return
        AnsiConsole.MarkupLine("\n[grey]Press any key to return...[/]");
        Console.ReadKey(true);
    }

    // ---------------- Helper Methods ----------------

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

        // Find the session
        var session = sessions.First(s => s.Id == id);

        // Prompt for new start time
        DateTime newStart;
        while (true)
        {
            var input = AnsiConsole.Ask<string>("Enter new start time (dd-MM-yy HH:mm):");
            if (DateTime.TryParseExact(input, "dd-MM-yy HH:mm", null, System.Globalization.DateTimeStyles.None, out newStart))
                break;

            AnsiConsole.MarkupLine("[red]Invalid format. Please use dd-MM-yy HH:mm[/]");
        }

        DateTime newEnd;
        while (true)
        {
            var input = AnsiConsole.Ask<string>("Enter new end time (dd-MM-yy HH:mm):");
            if (DateTime.TryParseExact(input, "dd-MM-yy HH:mm", null, System.Globalization.DateTimeStyles.None, out newEnd) && newEnd >= newStart)
                break;

            AnsiConsole.MarkupLine("[red]Invalid format or end is before start. Try again.[/]");
        }

        // Call the service layer to update
        _service.UpdateSession(id, newStart, newEnd);

        AnsiConsole.MarkupLine($"[green]Session {id} updated successfully![/]");
    }

    private void DeleteSessions(List<CodingSession> sessions)
    {
        // Example: prompt for ID to delete, then call _service.DeleteSession(...)

        int id = AnsiConsole.Prompt(
        new SelectionPrompt<int>()
            .Title("[yellow]Select the ID of the session to Delete[/]")
            .AddChoices(sessions.Select(s => s.Id))
    );
        _service.DeleteSession(id);
        AnsiConsole.MarkupLine($"[yellow]Session {id} updated successfully![/]");
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

    public void BuildTable(List<CodingSession>? sessions, bool includeTotal = false)
    {
        Console.Clear();
        if (sessions == null || !sessions.Any()) return;

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
            table.AddRow("[yellow]TOTAL[/]", "", "", "", "", $"[yellow]{total:hh\\:mm\\:ss}[/]");
        }

        // Print the table
        AnsiConsole.Write(table);
    }

    public void ChooseInsert()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Choose insert method[/]")
                .AddChoices(new[] { "Stopwatch", "Insert manually" })
        );

        DateTime startTime;
        DateTime endTime;

        if (choice == "Stopwatch")
        {
            startTime = DateTime.Now;
            AnsiConsole.MarkupLine("[green]Stopwatch started. Press any key to stop...[/]");

            // Show ticking timer (optional)
            while (!Console.KeyAvailable)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write($"Elapsed: {(DateTime.Now - startTime):hh\\:mm\\:ss}");
                Thread.Sleep(200);
            }
            Console.ReadKey(true); // consume the key
            endTime = DateTime.Now;
        }
        else
        {
            // Manual entry
            startTime = PromptForDateTime("Enter start time (dd-MM-yy HH:mm):");
            while (true)
            {
                endTime = PromptForDateTime("Enter end time (dd-MM-yy HH:mm):");
                if (endTime >= startTime) break;
                AnsiConsole.MarkupLine("[red]End time cannot be before start time. Try again.[/]");
            }
        }

        // Create session object
        var session = new CodingSession(0, startTime, endTime); // Id will be auto-assigned in DB
        _service.AddSession(session);
        AnsiConsole.MarkupLine("[green]Session added successfully![/]");
    }

    private DateTime PromptForDateTime(string prompt)
    {
        while (true)
        {
            var input = AnsiConsole.Ask<string>(prompt);
            if (DateTime.TryParseExact(input, "dd-MM-yy HH:mm", null, System.Globalization.DateTimeStyles.None, out var dt))
                return dt;

            AnsiConsole.MarkupLine("[red]Invalid format. Use dd-MM-yy HH:mm[/]");
        }
    }



}
