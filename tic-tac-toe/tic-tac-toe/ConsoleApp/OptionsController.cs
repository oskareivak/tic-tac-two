using ConsoleApp;
using DAL;
using GameBrain;
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
        // TODO: remove User shouldn't actually reach this statement, but just in case (temporary precaution).
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
                Title = gameNames[i].Split("|")[0] + "|" + gameNames[i].Split("|")[1],
                // Title = gameNames[i],
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
        // TODO: remove User shouldn't actually reach this statement, but just in case (temporary precaution).
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
                Title = gameNames[i].Split("|")[0] + "|" + gameNames[i].Split("|")[1],
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
                Title = configRepository.GetConfigurationNames()[i].Split("|").First().Trim(),
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
        var gameModes = new List<EGameMode> { EGameMode.PvP, EGameMode.PvAi, EGameMode.AivAi };
        
        for (int i = 0; i < gameModes.Count; i++)
        {
            var returnValue = gameModes[i].ToString();
            gameModeMenuItems.Add(new MenuItem()
            {
                Title = Settings.GameModeStrings[gameModes[i]],
                Shortcut = (i+1).ToString(),
                MenuItemAction = () => returnValue
            });
        }
        
        var gameModeMenu = new Menu(EMenuLevel.Secondary,
            "TIC-TAC-TWO - choose game mode",
            gameModeMenuItems,
            isCustomMenu: true
        );
        // var chosenGameMode = gameModeMenu.Run();
        // if (chosenGameMode == "R")
        // {
        //     return "R";
        // }
        // if (chosenGameMode == "E")
        // {
        //     return "E";
        // }

        // GameController.MainLoop(null, null, Enum.Parse<EGameMode>(chosenGameMode));
        return gameModeMenu.Run();
    }
    
    public static string DeleteConfiguration()
    {
        ConfigRepositoryDb configRepositoryDb;
        
        if (Settings.Mode == ESavingMode.Database)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite($"Data Source={FileHelper.BasePath}app.db");
            var context = new AppDbContext(optionsBuilder.Options);

            configRepositoryDb = new ConfigRepositoryDb(context);
        }
        
        
        // TODO: remove User shouldn't actually reach this statement, but just in case (temporary precaution).
        if (Settings.Mode != ESavingMode.Json && Settings.Mode != ESavingMode.Database)
        {
            Console.WriteLine("Deleting configurations is not supported in in-memory mode.");
            return "";
        }
        
        var configMenuItems = new List<MenuItem>();
        var configRepositoryInMemory = new ConfigRepositoryInMemory();
        List<string> configNames;

        if (Settings.Mode == ESavingMode.Database)
        {
            configNames = configRepositoryDb.GetConfigurationNames();
        }
        else
        {
            configNames = ConfigRepository.GetConfigurationNames();
        }
        
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
                Title = configNames[i].Split("|").First().Trim(),
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
            if (Settings.Mode == ESavingMode.Database)
            {
                configRepositoryDb.DeleteConfiguration(chosenConfigName);
            }
            else
            {
                ConfigRepository.DeleteConfiguration(chosenConfigName);
            }
            Console.WriteLine("Configuration deleted successfully.");
        }
        catch (DbUpdateConcurrencyException)
        {
            Console.WriteLine("The configuration could not be deleted because it was modified or deleted by another process.");
        }

        return "";
    }
    
    
}