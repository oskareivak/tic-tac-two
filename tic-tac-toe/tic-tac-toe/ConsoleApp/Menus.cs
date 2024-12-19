using Common;
using DAL;
using MenuSystem;

namespace ConsoleApp;

public static class Menus
{
    private static readonly ConfigRepositoryInMemory ConfigRepositoryInMemory = new ConfigRepositoryInMemory();
    
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
            new MenuItem() 
            {
                Shortcut = "DC",
                Title = "Delete a game configuration",
                MenuItemAction = OptionsController.DeleteConfiguration
            },
            new MenuItem()
            {
                Shortcut = "DG",
                Title = "Delete a saved game",
                MenuItemAction = OptionsController.DeleteSavedGame
            }
        }
    );
    
    public static Menu MainMenu = new Menu(
        EMenuLevel.Main,
        "TIC-TAC-TWO", new List<MenuItem>
        {
            new MenuItem()
            {
                Shortcut = "N",
                Title = "New game",
                MenuItemAction = () => GameController.MainLoop(null, null)
            },
            new MenuItem()
            {
                Shortcut = "L",
                Title = "Load game",
                MenuItemAction = OptionsController.LoadGame
            },
            new MenuItem()
            {
                Shortcut = "O",
                Title = "Options",
                MenuItemAction = OptionsMenu.Run
            }
        }
    );
}