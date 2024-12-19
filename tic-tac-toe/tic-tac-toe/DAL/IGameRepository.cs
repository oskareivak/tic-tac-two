using Domain;
using GameBrain;

namespace DAL;

public interface IGameRepository
{
    public List<string> GetGameNames();
    public List<string> GetGameNamesForUser(string username);
    public GameState GetGameByName(string name);
    public void DeleteGame(string name);
    public GameState GetGameById(int gameId); 
    public void DeleteGameById(int gameId);
    
    public int SaveGameReturnId(string jsonStateString, string gameConfigName);

    public Dictionary<int, string> GetGameIdNamePairsForUser(string username);
    
    public void UpdateGame(int gameId, string gameStateJson);

    bool CanBeDeletedWeb(int gameId, string userName);
}