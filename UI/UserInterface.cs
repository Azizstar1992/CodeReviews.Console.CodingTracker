using Spectre.Console;
using Microsoft.Data.Sqlite;
internal class UserInterface
{
    internal static void MainMenu()
    {



        var repository = new SessionRepository();
        var service = new SessionService(repository);


        var sessionMenuUI = new SessionMenuUI(service);
    
        
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

    

}
