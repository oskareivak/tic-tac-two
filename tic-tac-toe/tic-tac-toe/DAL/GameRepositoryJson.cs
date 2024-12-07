using Domain;
using GameBrain;

namespace DAL;

public class GameRepositoryJson : IGameRepository
{
    public bool SaveGame(string jsonStateString, string gameConfigName)
    {
        var savedGames = Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.GameExtension);
        if (savedGames.Length >= 100)
        {
            return false;
        }
        
        var fileName = FileHelper.BasePath + 
                       gameConfigName + " | " + 
                       DateTime.Now.ToString("f") + 
                       FileHelper.GameExtension;
        
        File.WriteAllText(fileName, jsonStateString);
        return true;
    }

    public List<string> GetGameNames()
    {   
        if (!Directory.Exists(FileHelper.BasePath))
        {
            Console.WriteLine("You don't have any saved configurations yet.");
            return null!;
        }
        
        var result = new List<string>();
        foreach (var fullFileName in Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.GameExtension))
        {
            var fileNameParts = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fullFileName));
            result.Add(fileNameParts);
        }

        return result;
    }
    
    public GameState GetGameByName(string name)
    {
        var gameJsonStr = File.ReadAllText(FileHelper.BasePath + name + FileHelper.GameExtension);
        var gameState = System.Text.Json.JsonSerializer.Deserialize<GameState>(gameJsonStr);
        return gameState!;
    }
    
    public void DeleteGame(string name)
    {
        var fileToDelete = FileHelper.BasePath + name + FileHelper.GameExtension;
        if (File.Exists(fileToDelete))
        {
            File.Delete(fileToDelete);
        }
        else
        {
            Console.WriteLine("Cannot find the file to delete.");
        }
    }

    public SavedGame GetGameById(int gameId)
    {
        throw new NotImplementedException();
    }

    public void DeleteGameById(int gameId)
    {
        throw new NotImplementedException();
    }

    public int SaveGameReturnId(string jsonStateString, string gameConfigName)
    {
        throw new NotImplementedException();
    }
}