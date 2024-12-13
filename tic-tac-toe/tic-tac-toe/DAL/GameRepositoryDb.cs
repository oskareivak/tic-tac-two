using Domain;
using GameBrain;
using Microsoft.EntityFrameworkCore;

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
        if (_context.SavedGames.Count() >= 100)
        {
            return false;
        } 
        
        var config = _context.Configurations
            .FirstOrDefault(c => c.Name == gameConfigName);

        if (config == null)
        {
            throw new Exception("Configuration not found");
        }

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
            .Select(g => $"{g.Configuration.Name} | {g.CreatedAtDateTime}")
            .ToList();
    }

    public GameState GetGameByName(string name)
    {
        var parts = name.Split(" | ");
        if (parts.Length != 2) throw new Exception("Invalid game name format");

        var configName = parts[0];
        var createdAt = parts[1];

        var game = _context.SavedGames
            .FirstOrDefault(g => g.Configuration.Name == configName && g.CreatedAtDateTime == createdAt);

        if (game == null) throw new Exception("Game not found");

        if (string.IsNullOrEmpty(game.State)) throw new Exception("Game state is null or empty");

        return System.Text.Json.JsonSerializer.Deserialize<GameState>(game.State) ?? throw new Exception("Deserialization failed");
    }

    public void DeleteGame(string name)
    {
        var parts = name.Split(" | ");
        if (parts.Length != 2) throw new Exception("Invalid game name format");

        var configName = parts[0];
        var createdAt = parts[1];

        var game = _context.SavedGames
            .FirstOrDefault(g => g.Configuration.Name == configName && g.CreatedAtDateTime == createdAt);

        if (game == null) throw new Exception("Game not found");

        _context.SavedGames.Remove(game);
        _context.SaveChanges();
    }

    public GameState GetGameById(int gameId)
    {
        var savedGame = _context.SavedGames.First(g => g.Id == gameId);
        
        if (savedGame == null) throw new Exception("Game not found");

        if (string.IsNullOrEmpty(savedGame.State)) throw new Exception("Game state is null or empty");

        return System.Text.Json.JsonSerializer.Deserialize<GameState>(savedGame.State) ?? throw new Exception("Deserialization failed");
    }

    public void DeleteGameById(int gameId)
    {
        var game = _context.SavedGames
            .FirstOrDefault(g => g.Id == gameId);

        if (game != null)
        {
            _context.SavedGames.Remove(game);
            _context.SaveChanges();
        }

        Console.WriteLine($"Game with id [{gameId}] not found at DeleteGameById");
        // throw new Exception("Game not found");
    }
    
    public int SaveGameReturnId(string jsonStateString, string gameConfigName)
    {   
        // if (_context.SavedGames.Count() >= 100)
        // {
        //     return null;
        // }  TODO: Implement this check in web and check console implementation
    
        var config = _context.Configurations
            .FirstOrDefault(c => c.Name == gameConfigName);

        if (config == null)
        {
            throw new Exception("Configuration not found");
        }

        var newGame = new SavedGame
        {
            CreatedAtDateTime = DateTime.Now.ToString("f"),
            State = jsonStateString,
            ConfigurationId = config.Id
        };

        _context.SavedGames.Add(newGame);
        _context.SaveChanges();
        
        return newGame.Id;
    }
    
    public Dictionary<int, string> GetGameIdNamePairs()
    {
        return _context.SavedGames
            .Include(c => c.Configuration) // Ensure Configuration is eagerly loaded
            .Where(c => c.Configuration != null)
            .OrderBy(c => c.Configuration!.Name)
            .ToDictionary(c => c.Id, c => c.Configuration!.Name + " | " + c.CreatedAtDateTime);
    }
}