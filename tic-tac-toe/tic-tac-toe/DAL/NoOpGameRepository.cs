using Domain;
using GameBrain;

namespace DAL;

public class NoOpGameRepository : IGameRepository
{
    public List<string> GetGameNames() => new List<string>();
    public bool SaveGame(string gameStateJson, string gameConfigName) => false;
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

    public Dictionary<int, string> GetGameIdNamePairs()
    {
        throw new NotImplementedException();
    }

    public GameState? GetGameByName(string gameName) => null;
    
    public SavedGame GetSavedGame(int gameId)
    {
        throw new NotImplementedException();
    }
    
}