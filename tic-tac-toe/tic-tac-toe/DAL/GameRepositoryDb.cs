using Domain;
using GameBrain;

namespace DAL;

public class GameRepositoryDb : IGameRepository
{
    private readonly AppDbContext _context;

    public GameRepositoryDb(AppDbContext context)
    {
        _context = context;
    }
    
    public bool SaveGame(string jsonStateString, string gameConfigName)
    {
        var config = _context.Configurations
            .FirstOrDefault(c => c.Name == gameConfigName);

        if (config == null) throw new Exception("Configuration not found");

        var newGame = new SavedGame
        {
            CreatedAtDateTime = DateTime.Now.ToString("f"),
            State = jsonStateString,
            ConfigurationId = config.Id
        };

        _context.SavedGames.Add(newGame);
        _context.SaveChanges();
        return true;
    }
    
    public List<string> GetGameNames()
    {
        return _context.SavedGames
            .OrderBy(g => g.CreatedAtDateTime)
            .Select(g => g.CreatedAtDateTime)
            .ToList();
    }
    
    public GameState GetGameByName(string name)
    {
        var game = _context.SavedGames
            .FirstOrDefault(g => g.CreatedAtDateTime == name);

        if (game == null) throw new Exception("Game not found");

        if (string.IsNullOrEmpty(game.State)) throw new Exception("Game state is null or empty");

        return System.Text.Json.JsonSerializer.Deserialize<GameState>(game.State) ?? throw new Exception("Deserialization failed");
    }
    
    public void DeleteGame(string name){
        var game = _context.SavedGames
            .FirstOrDefault(g => g.CreatedAtDateTime == name);

        if (game == null) throw new Exception("Game not found");

        _context.SavedGames.Remove(game);
        _context.SaveChanges();
    }
}