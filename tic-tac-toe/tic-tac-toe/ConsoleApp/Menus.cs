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
            },
            new MenuItem()
            {
                Shortcut = "C",
                Title = "Make a new game configuration",
                MenuItemAction = DummyMethod
            }
        ]);

    public static Menu MainMenu = new Menu(
        EMenuLevel.Main,
        "TIC-TAC-TOE", menuItems: [
            new MenuItem()
            {
                Shortcut = "N",
                Title = "New game",
                MenuItemAction = GameController.MainLoop
                // MenuItemAction = NewGame
            },
            new MenuItem()
            {
                Shortcut = "O",
                Title = "Options",
                MenuItemAction = () => OptionsMenu.Run(new Stack<Menu>())
            }
        ]);

    private static string DummyMethod()
    {
        Console.Write("This is a dummy method - press any key to exit...");
        Console.ReadKey();
        return "Exiting...";
    }
}