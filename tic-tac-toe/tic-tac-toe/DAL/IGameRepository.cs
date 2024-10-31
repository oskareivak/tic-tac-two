using GameBrain;

namespace DAL;

public interface IGameRepository
{
    public void SaveGame(string jsonStateString, string gameConfigName);
    public List<string> GetGameNames();
    public GameState GetGameByName(string name);
}