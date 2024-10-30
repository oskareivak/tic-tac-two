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
}