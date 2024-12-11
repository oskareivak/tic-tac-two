using Domain;
using GameBrain;

namespace DAL;

public interface IGameRepository
{
    public bool SaveGame(string jsonStateString, string gameConfigName);
    public List<string> GetGameNames();
    public GameState GetGameByName(string name);
    public void DeleteGame(string name);
    public GameState GetGameById(int gameId); // TODO: make this return gamestate
    public void DeleteGameById(int gameId);
    
    public int SaveGameReturnId(string jsonStateString, string gameConfigName);

    public Dictionary<int, string> GetGameIdNamePairs();
}