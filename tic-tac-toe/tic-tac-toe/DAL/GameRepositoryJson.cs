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
            Console.WriteLine("You don't have any saved games yet.");
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

    public List<string> GetGameNamesForUser(string username)
    {
        if (!Directory.Exists(FileHelper.BasePath))
        {
            Console.WriteLine("You don't have any saved games yet.");
            return null!;
        }
        
        var result = new List<string>();

        foreach (var fullFileName in Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.GameExtension))
        {
            var fileNameParts = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fullFileName));
            
            var gameState = GetGameByName(fileNameParts);
            if (gameState.XPlayerUsername == username || gameState.OPlayerUsername == username)
            {
                result.Add(fileNameParts + " | " + gameState.XPlayerUsername + " VS " + gameState.OPlayerUsername);
            }
        }

        return result;
    }
    
    public GameState GetGameByName(string name)
    {
        
        // FYI: Uses the game ID inside the name as well, so not just by name.
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

    public GameState GetGameById(int gameId)
    {
        var data = Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.GameExtension)
            .Select(Path.GetFileNameWithoutExtension)
            .Select(Path.GetFileNameWithoutExtension)
            .ToList();
        
        foreach (var gameNameWithId in data)
        {
            if (gameNameWithId!.Split("|").Last().Trim() == gameId.ToString())
            {
                var gameJsonStr = File.ReadAllText(FileHelper.BasePath + gameNameWithId + FileHelper.GameExtension);
                var gameState = System.Text.Json.JsonSerializer.Deserialize<GameState>(gameJsonStr);
                return gameState!;
            }
        }
        
        throw new Exception($"Game not found with id: {gameId}.");
    }

    public void DeleteGameById(int gameId)
    {
        var data = Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.GameExtension)
            .ToList();

        var games = new Dictionary<int, string>();
        
        foreach (var gameFile in data)
        {
            var fileName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(gameFile));
            var id = int.Parse(fileName.Split("|").Last().Trim());
            
            var gameJsonStr = File.ReadAllText(gameFile);
            var gameState = System.Text.Json.JsonSerializer.Deserialize<GameState>(gameJsonStr);
            var name = fileName.Split("|")[0] + "|" + fileName.Split("|")[1];
            
            games.Add(id, name);
        }
        
        
        if (!games.ContainsKey(gameId))
        {
            throw new Exception($"Game not found with id: {gameId}.");
        }
        
        var gameName = games[gameId];
        
        var fileToDelete = FileHelper.BasePath + gameName + "| " + gameId + FileHelper.GameExtension;
        
        if (File.Exists(fileToDelete))
        {
            File.Delete(fileToDelete);
        }
        else
        {   
            throw new Exception($"Game not found with id: {gameId}.");
        }
    }

    public int SaveGameReturnId(string jsonStateString, string gameConfigName)
    {
        var data = Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.GameExtension)
            .Select(Path.GetFileNameWithoutExtension)
            .Select(Path.GetFileNameWithoutExtension)
            .ToList();
        
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
        
        return newId;
    }

    public Dictionary<int, string> GetGameIdNamePairs(string username)
    {
        
        var data = Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.GameExtension)
            .ToList();

        var idNamePairs = new Dictionary<int, string>();
        
        foreach (var gameFile in data)
        {
            var fileName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(gameFile));
            var id = int.Parse(fileName.Split("|").Last().Trim());
            
            var gameJsonStr = File.ReadAllText(gameFile);
            var gameState = System.Text.Json.JsonSerializer.Deserialize<GameState>(gameJsonStr);
            
            
            
            if (gameState!.XPlayerUsername == username || gameState.OPlayerUsername == username)
            {
                var name = fileName.Split("|")[0] + "|" + fileName.Split("|")[1] + " | " +  
                           gameState.XPlayerUsername + " VS " + gameState.OPlayerUsername;
                idNamePairs.Add(id, name);
            }
        }

        return idNamePairs;
    }
}