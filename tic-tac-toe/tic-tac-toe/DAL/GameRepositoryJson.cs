using GameBrain;

namespace DAL;

public class GameRepositoryJson : IGameRepository
{
    
    
    public void SaveGame(string jsonStateString, string gameConfigName)
    {
        var fileName = FileHelper.BasePath + 
                       gameConfigName+ " " + 
                       DateTime.Now.ToString("O") + 
                       FileHelper.GameExtension;
        
        File.WriteAllText(fileName, jsonStateString);
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
}