var repository = new SessionRepository();
var service = new SessionService(repository);
var ui = new UserInterface(service);

ui.MainMenu();