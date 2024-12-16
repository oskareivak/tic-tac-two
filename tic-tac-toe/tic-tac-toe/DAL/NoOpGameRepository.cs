using Domain;
using GameBrain;

namespace DAL;

public class NoOpGameRepository : IGameRepository
{
    public bool SaveGame(string gameStateJson, string gameConfigName) => false;
    public List<string> GetGameNames()
    {
        throw new NotImplementedException();
    }

    public List<string> GetGameNamesForUser(string username)
    {
        throw new NotImplementedException();
    }
    
    public void DeleteGame(string gameName) { }
    public GameState GetGameById(int gameId)
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

    public Dictionary<int, string> GetGameIdNamePairs(string username)
    {
        throw new NotImplementedException();
    }

    public void UpdateGame(int gameId, string gameStateJson)
    {
        throw new NotImplementedException();
    }

    public Dictionary<int, string> GetGameIdNamePairs()
    {
        throw new NotImplementedException();
    }

    public GameState GetGameByName(string gameName)
    {
        throw new NotImplementedException();
    }
    
    public SavedGame GetSavedGame(int gameId)
    {
        throw new NotImplementedException();
    }
    
}