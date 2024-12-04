using Domain;
using GameBrain;

namespace DAL;

public interface IGameRepository
{
    public bool SaveGame(string jsonStateString, string gameConfigName);
    public List<string> GetGameNames();
    public GameState GetGameByName(string name);
    public void DeleteGame(string name);
    public SavedGame GetSavedGame(int gameId);
}