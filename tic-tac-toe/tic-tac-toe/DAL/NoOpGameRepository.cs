using GameBrain;

namespace DAL;

public class NoOpGameRepository : IGameRepository
{
    public List<string> GetGameNames() => new List<string>();
    public bool SaveGame(string gameStateJson, string gameConfigName) => false;
    public void DeleteGame(string gameName) { }
    public GameState? GetGameByName(string gameName) => null;
}