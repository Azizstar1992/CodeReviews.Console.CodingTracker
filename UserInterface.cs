using Spectre.Console;

internal class UserInterface
{
    internal static void MainMenu()
    {
        var choice = AnsiConsole.Prompt(new SelectionPrompt<MenuOption>().Title("[yellow]Please select a choice[/]").AddChoices(Enum.GetValues<MenuOption>()));
        bool running = true;
        while (running)
            switch (choice)
            {
                case MenuOption.ViewSessions:
                    ShowViewSessionsMenu();
                    break;

                case MenuOption.AddSession:
                    ShowAddSessionMenu();
                    break;

                case MenuOption.UpdateSession:
                    ShowUpdateSessionMenu();
                    break;

                case MenuOption.DeleteSession:
                    ShowDeleteSessionMenu();
                    break;

                case MenuOption.Exit:
                    running = false;
                    return;
            }
    }

}
}