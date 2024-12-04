using DAL;
using MenuSystem;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp;

public class OptionsController
{   
    private static readonly IConfigRepository ConfigRepository;
    private static readonly IGameRepository GameRepository;
    
    static OptionsController()
    {
        if (Settings.Mode == ESavingMode.Json)
        {
            ConfigRepository = new ConfigRepositoryJson();
            GameRepository = new GameRepositoryJson();
        }
        if (Settings.Mode == ESavingMode.Database)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite($"Data Source={FileHelper.BasePath}app.db");
            var context = new AppDbContext(optionsBuilder.Options);

            ConfigRepository = new ConfigRepositoryDb(context);
            GameRepository = new GameRepositoryDb(context);
        }
        if (Settings.Mode == ESavingMode.Memory)
        {
            ConfigRepository = new ConfigRepositoryInMemory();
            GameRepository = new NoOpGameRepository();
        }
    }
    
    public static string LoadGame()
    {   
        // User shouldn't actually reach this statement, but just in case (temporary precaution).
        if (Settings.Mode != ESavingMode.Json && Settings.Mode != ESavingMode.Database)
        {
            Console.WriteLine("Loading games is not supported in in-memory mode.");
            return "";
        }
        
        var gameMenuItems = new List<MenuItem>();
        var gameNames = GameRepository.GetGameNames();
        
        if (gameNames.Count == 0)
        {
            Console.WriteLine("\nYou don't have any saved games yet.");
            return "";
        }
        
        for (int i = 0; i < gameNames.Count; i++)
        {
            var returnValue = gameNames[i];
            gameMenuItems.Add(new MenuItem()
            {
                Title = gameNames[i],
                Shortcut = (i+1).ToString(),
                MenuItemAction = () => returnValue
            });
        }

        var gameMenu = new Menu(EMenuLevel.Secondary,
            "TIC-TAC-TWO - choose game to load",
            gameMenuItems,
            isCustomMenu: true
        );

        var chosenGameName = gameMenu.Run();
        if (chosenGameName == "R")
        {
            return "R";
        }
        if (chosenGameName == "E")
        {
            return "E";
        }
        
        GameController.MainLoop(GameRepository.GetGameByName(chosenGameName), chosenGameName);
        
        return "";
    }

    public static string DeleteSavedGame()
    {   
        // User shouldn't actually reach this statement, but just in case (temporary precaution).
        if (Settings.Mode != ESavingMode.Json && Settings.Mode != ESavingMode.Database)
        {
            Console.WriteLine("Deleting saved games is not supported in in-memory mode.");
            return "";
        }
        
        var gameMenuItems = new List<MenuItem>();
        var gameNames = GameRepository.GetGameNames();

        if (gameNames.Count == 0)
        {
            Console.WriteLine("\nYou don't have any saved games yet.");
            return "";
        }

        for (int i = 0; i < gameNames.Count; i++)
        {
            var returnValue = gameNames[i];
            gameMenuItems.Add(new MenuItem()
            {
                Title = gameNames[i],
                Shortcut = (i + 1).ToString(),
                MenuItemAction = () => returnValue
            });
        }

        var gameMenu = new Menu(EMenuLevel.Deep,
            "TIC-TAC-TWO - choose game to delete",
            gameMenuItems,
            isCustomMenu: true
        );

        var chosenGameName = gameMenu.Run();
        if (chosenGameName == "R")
        {
            return "R";
        }
        if (chosenGameName == "E")
        {
            return "E";
        }
        if (chosenGameName == "M")
        {
            return "M";
        }
        
        GameRepository.DeleteGame(chosenGameName);
        Console.WriteLine("\nGame deleted successfully.");

        return "";
    }
    
    public static string ChooseConfiguration(IConfigRepository configRepository)
    {
        var configMenuItems = new List<MenuItem>();

        for (int i = 0; i < configRepository.GetConfigurationNames().Count; i++)
        {
            var returnValue = i.ToString();
            configMenuItems.Add(new MenuItem()
            {
                Title = configRepository.GetConfigurationNames()[i],
                Shortcut = (i+1).ToString(),
                MenuItemAction = () => returnValue
            });
        }

        var configMenu = new Menu(EMenuLevel.Secondary,
            "TIC-TAC-TWO - choose game config",
            configMenuItems,
            isCustomMenu: true
        );
        
        return configMenu.Run();
    }

    public static string ChooseGamemode()
    {
        var gameModeMenuItems = new List<MenuItem>();
        var gameModes = new List<string> { "PvP", "PvAI", "AIvAI" };
        
        for (int i = 0; i < gameModes.Count; i++)
        {
            var returnValue = gameModes[i];
            gameModeMenuItems.Add(new MenuItem()
            {
                Title = gameModes[i],
                Shortcut = (i+1).ToString(),
                MenuItemAction = () => returnValue
            });
        }
        
        var gameModeMenu = new Menu(EMenuLevel.Secondary,
            "TIC-TAC-TWO - choose game mode",
            gameModeMenuItems,
            isCustomMenu: true
        );
        var chosenGameMode = gameModeMenu.Run();
        if (chosenGameMode == "R")
        {
            return "R";
        }
        if (chosenGameMode == "E")
        {
            return "E";
        }

        GameController.MainLoop(null, null, chosenGameMode);
        return "";
    }
    
    public static string DeleteConfiguration()
    {   
        // User shouldn't actually reach this statement, but just in case (temporary precaution).
        if (Settings.Mode != ESavingMode.Json && Settings.Mode != ESavingMode.Database)
        {
            Console.WriteLine("Deleting configurations is not supported in in-memory mode.");
            return "";
        }
        
        var configMenuItems = new List<MenuItem>();
        var configRepositoryInMemory = new ConfigRepositoryInMemory();
        var configNames = ConfigRepository.GetConfigurationNames();
        var defaultConfigurations = configRepositoryInMemory.GetConfigurationNames();
        
        if (configNames.Count <= defaultConfigurations.Count) 
        {
            Console.WriteLine("\nYou don't have any configurations to delete yet.");
            return "";
        }
        
        for (int i = 0; i < configNames.Count; i++)
        {
            var returnValue = configNames[i];
            configMenuItems.Add(new MenuItem()
            {
                Title = configNames[i],
                Shortcut = (i+1).ToString(),
                MenuItemAction = () => returnValue
            });
        }

        var configMenu = new Menu(EMenuLevel.Deep,
            "TIC-TAC-TWO - delete game configuraton (Deleting a game configuration also deletes all saved games using it.)",
            configMenuItems,
            isCustomMenu: true
        );
        
        var chosenConfigName = configMenu.Run();
        if (chosenConfigName == "R")
        {
            return "R";
        }
        if (chosenConfigName == "E")
        {
            return "E";
        }
        if (chosenConfigName == "M")
        {
            return "M";
        }
        
        if (defaultConfigurations.Contains(chosenConfigName))
        {
            Console.WriteLine("\nYou cannot delete default configurations!");
            return "";
        }
        
        try
        {
            ConfigRepository.DeleteConfiguration(chosenConfigName);
            Console.WriteLine("Configuration deleted successfully.");
        }
        catch (DbUpdateConcurrencyException)
        {
            Console.WriteLine("The configuration could not be deleted because it was modified or deleted by another process.");
        }

        return "";
    }
    
    
}