using Spectre.Console;
using Microsoft.Data.Sqlite;
internal class UserInterface
{
    internal static void MainMenu()
    {



        var repository = new SessionRepository();
        var service = new SessionService(repository);


        var sessionMenuUI = new SessionMenuUI(service);
        SeedRandomSessions(4);
        bool running = true;
        while (running)
        {
            var choice = AnsiConsole.Prompt(new SelectionPrompt<MenuOption>().Title("[yellow]Please select a choice[/]").AddChoices(Enum.GetValues<MenuOption>()));

            switch (choice)
            {
                case MenuOption.GenerateReport:
                    sessionMenuUI.ShowByMonth("report");
                    break;
                case MenuOption.ViewSessions:
                    sessionMenuUI.ShowByMonth("show");
                    break;

                case MenuOption.AddSession:
                    sessionMenuUI.ChooseInsert();
                    break;

                case MenuOption.UpdateSession:
                     sessionMenuUI.ShowByMonth("update");
                    break;

                case MenuOption.DeleteSession:
                    sessionMenuUI.ShowByMonth("delete");
                    break;

                case MenuOption.Exit:
                    running = false;
                    return;
            }
        }

    }

    public static void SeedRandomSessions(int count = 10)
    {
        var random = new Random();

        using var connection = new SqliteConnection("Data Source=Database/CodingTracker.db");
        connection.Open();


        var tableCmd = connection.CreateCommand();
        tableCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS CodingSessions (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                StartTime TEXT NOT NULL,
                EndTime TEXT NOT NULL,
                DurationMinutes INTEGER NOT NULL
            );
        ";
        tableCmd.ExecuteNonQuery();
        for (int i = 0; i < count; i++)
        {
            DateTime start = DateTime.Now.AddDays(-random.Next(200, 700));
            DateTime end = start.AddMinutes(random.Next(10, 300));

            int durationMinutes = (int)(end - start).TotalMinutes;

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
            INSERT INTO CodingSessions (StartTime, EndTime, DurationMinutes)
            VALUES ($start, $end, $duration)
        ";

            cmd.Parameters.AddWithValue("$start", start.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("$end", end.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("$duration", durationMinutes);

            cmd.ExecuteNonQuery();
        }
    }

}
