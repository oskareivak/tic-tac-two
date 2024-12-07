using Domain;
using GameBrain;

namespace DAL;

public interface IGameRepository
{
    public bool SaveGame(string jsonStateString, string gameConfigName);
    public List<string> GetGameNames();
    public GameState GetGameByName(string name);
    public void DeleteGame(string name);
    public SavedGame GetGameById(int gameId);
    public void DeleteGameById(int gameId);
    
    public int SaveGameReturnId(string jsonStateString, string gameConfigName);
}