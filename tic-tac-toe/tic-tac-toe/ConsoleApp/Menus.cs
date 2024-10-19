using MenuSystem;

namespace ConsoleApp;

public static class Menus
{
    public static readonly Menu OptionsMenu =
        new Menu(
        EMenuLevel.Secondary,
        "TIC-TAC-TOE Options", menuItems: [
            new MenuItem()
            {
                Shortcut = "X",
                Title = "X Starts",
                MenuItemAction = DummyMethod
            },
            new MenuItem()
            {
                Shortcut = "O",
                Title = "O Starts",
                MenuItemAction = DummyMethod
            }
        ]);

    public static Menu MainMenu = new Menu(
        EMenuLevel.Main,
        "TIC-TAC-TOE", menuItems: [
            new MenuItem()
            {
                Shortcut = "O",
                Title = "Options",
                MenuItemAction = OptionsMenu.Run
            },
            new MenuItem()
            {
                Shortcut = "N",
                Title = "New game",
                MenuItemAction = GameController.MainLoop
                // MenuItemAction = NewGame
            }
        ]);

    private static string DummyMethod()
    {
        Console.Write("This is a dummy method - press any key to exit...");
        Console.ReadKey();
        return "Exiting...";
    }
}