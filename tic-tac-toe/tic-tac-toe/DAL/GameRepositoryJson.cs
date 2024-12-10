using System.Text.Json;
using Domain;
using GameBrain;

namespace DAL;

public class GameRepositoryJson : IGameRepository
{
    public bool SaveGame(string jsonStateString, string gameConfigName)
    {
        var data = Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.GameExtension)
            .Select(Path.GetFileNameWithoutExtension)
            .Select(Path.GetFileNameWithoutExtension)
            .ToList();
        
        if (data.Count >= 100)
        {
            return false;
        }
        
        var existingIds = data
            .Select(game => game!.Split('|').Last())
            .Select(idStr => int.TryParse(idStr, out var id) ? id : (int?)null)
            .Where(id => id.HasValue)
            .Select(id => id.Value)
            .ToList();
        
        var newId = 1;
        while (existingIds.Contains(newId))
        {
            newId++;
        }
        
        var fileName = FileHelper.BasePath + $"{gameConfigName} | {DateTime.Now:f} | {newId}{FileHelper.GameExtension}";
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
            var final = fileNameParts.Split("|")[0] + "|" + fileNameParts.Split("|")[1];
            result.Add(final);
        }

        return result;
    }
    
    public GameState GetGameByName(string name)
    {
        
        // TODO: fix this
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

    public Dictionary<int, string> GetGameIdNamePairs()
    {
        throw new NotImplementedException();
    }
}