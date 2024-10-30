using System.Collections;
using DAL;
using GameBrain;
using MenuSystem;

namespace ConsoleApp;

public static class Menus
{
    
    private static readonly ConfigRepositoryInMemory ConfigRepositoryInMemory = new ConfigRepositoryInMemory();
    
    public static readonly Menu DeepMenu = new Menu(
        EMenuLevel.Deep,
        "TIC-TAC-TOE DEEP", [
            new MenuItem()
            {
                Shortcut = "T",
                Title = "To be implemented...",
            }
        ]
    );

    
    public static readonly Menu OptionsMenu =
        new Menu(
        EMenuLevel.Secondary,
        "TIC-TAC-TOE Options", menuItems: [
            new MenuItem()
            {
                Shortcut = "X",
                Title = "X Starts",
                MenuItemAction = DeepMenu.Run
            },
            new MenuItem()
            {
                Shortcut = "O",
                Title = "O Starts",
                MenuItemAction = DeepMenu.Run
            },
            new MenuItem()
            {
                Shortcut = "C",
                Title = "Make a new game configuration",
                MenuItemAction = GameController.NewConfiguration
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
            },
            new MenuItem()
            {
                Shortcut = "O",
                Title = "Options",
                MenuItemAction = OptionsMenu.Run
            }
        ]);

    private static string DummyMethod()
    {
        Console.Write("This is a dummy method - press any key to exit...");
        Console.ReadKey();
        return "Exiting...";
    }
}