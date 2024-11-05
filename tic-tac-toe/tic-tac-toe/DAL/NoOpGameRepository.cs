using GameBrain;

namespace DAL;

public class NoOpGameRepository : IGameRepository
{
    public List<string> GetGameNames() => new List<string>();
    public void SaveGame(string gameStateJson, string gameConfigName) { }
    public void DeleteGame(string gameName) { }
    public GameState? GetGameByName(string gameName) => null;
}