using GameBrain;

namespace DAL;

public class GameRepositoryDb : IGameRepository
{
    public bool SaveGame(string jsonStateString, string gameConfigName)
    {
        throw new System.NotImplementedException();
    }
    
    public List<string> GetGameNames()
    {
        throw new System.NotImplementedException();
    }
    
    public GameState GetGameByName(string name)
    {
        throw new System.NotImplementedException();
    }
    
    public void DeleteGame(string name){
        throw new System.NotImplementedException();
    }
}