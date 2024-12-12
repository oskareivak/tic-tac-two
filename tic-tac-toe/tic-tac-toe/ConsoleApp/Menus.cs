using DAL;
using MenuSystem;

namespace ConsoleApp;

public static class Menus
{
    
    private static readonly ConfigRepositoryInMemory ConfigRepositoryInMemory = new ConfigRepositoryInMemory();
    
    public static readonly Menu DeepMenu = new Menu(
        EMenuLevel.Deep,
        "TIC-TAC-TWO DEEP", [
            new MenuItem()
            {
                Shortcut = "T",
                Title = "To be implemented...",
            }
        ]
    );
    
    public static readonly Menu OptionsMenu = new Menu(
        EMenuLevel.Secondary,
        "TIC-TAC-TWO Options", new List<MenuItem>
        {
            new MenuItem()
            {
                Shortcut = "C",
                Title = "Make a new game configuration",
                MenuItemAction = GameController.NewConfiguration
            },
            Settings.Mode == ESavingMode.Json || Settings.Mode == ESavingMode.Database ? new MenuItem()
            {
                Shortcut = "DC",
                Title = "Delete a game configuration",
                MenuItemAction = OptionsController.DeleteConfiguration
            } : null,
            Settings.Mode == ESavingMode.Json || Settings.Mode == ESavingMode.Database ? new MenuItem()
            {
                Shortcut = "DG",
                Title = "Delete a saved game",
                MenuItemAction = OptionsController.DeleteSavedGame
            } : null
        }.Where(item => item != null).ToList()
    );
    
    public static Menu MainMenu = new Menu(
        EMenuLevel.Main,
        "TIC-TAC-TWO", new List<MenuItem>
        {
            new MenuItem()
            {
                Shortcut = "N",
                Title = "New game",
                // MenuItemAction = OptionsController.ChooseGamemode  TODO: Uncomment when AI is implemented
                MenuItemAction = () => GameController.MainLoop(null, null)
            },
            Settings.Mode == ESavingMode.Json || Settings.Mode == ESavingMode.Database ? new MenuItem()
            {
                Shortcut = "L",
                Title = "Load game",
                MenuItemAction = OptionsController.LoadGame
            } : null,
            new MenuItem()
            {
                Shortcut = "O",
                Title = "Options",
                MenuItemAction = OptionsMenu.Run
            }
        }.Where(item => item != null).ToList()
    );
    
    private static string DummyMethod()
    {
        Console.Write("This is a dummy method - press any key to exit...");
        Console.ReadKey();
        return "Exiting...";
    }
}