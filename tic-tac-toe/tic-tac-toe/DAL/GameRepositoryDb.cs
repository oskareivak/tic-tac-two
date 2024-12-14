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

    public List<string> GetGameNamesForUser(string username)
    {
        Console.WriteLine($"i am your username: {username}");
        var result = new List<string>();
        foreach (var game in _context.SavedGames)
        {   
            Console.WriteLine("i am a game in context.savedgames");
            var gameState = System.Text.Json.JsonSerializer.Deserialize<GameState>(game.State);
            if (gameState!.XPlayerUsername == username || gameState.OPlayerUsername == username)
            {   
                result.Add($"{gameState.GameConfiguration.Name} | {game.CreatedAtDateTime} | {game.Id} | " +
                           $"{gameState.XPlayerUsername} VS {gameState.OPlayerUsername}");
            }
        }
        Console.WriteLine($"i was called and returned {result.Count} games");

        return result;

        // return _context.SavedGames
        //     .OrderBy(g => g.CreatedAtDateTime)
        //     .Select(g => $"{g.Configuration.Name} | {g.CreatedAtDateTime}")
        //     .ToList();
    }

    public GameState GetGameByName(string name)
    {
        var parts = name.Split("|");
        if (parts.Length != 3) throw new Exception("Invalid game name format");

        var configName = parts[0].Trim();
        var createdAt = parts[1].Trim();
        var id = parts[2].Trim();

        var game = _context.SavedGames
            .FirstOrDefault(g => g.Configuration.Name == configName && g.CreatedAtDateTime == createdAt &&
                                 g.Id.ToString() == id);

        if (game == null) throw new Exception("Game not found");

        if (string.IsNullOrEmpty(game.State)) throw new Exception("Game state is null or empty");

        return System.Text.Json.JsonSerializer.Deserialize<GameState>(game.State) ?? throw new Exception("Deserialization failed");
    }

    public void DeleteGame(string name)
    {
        var parts = name.Split("|");
        if (parts.Length != 3) throw new Exception("Invalid game name format");

        var configName = parts[0].Trim();
        var createdAt = parts[1].Trim();
        var id = parts[2].Trim();

        var game = _context.SavedGames
            .FirstOrDefault(g => g.Configuration.Name == configName && g.CreatedAtDateTime == createdAt &&
                                 g.Id.ToString() == id);

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
    
    public Dictionary<int, string> GetGameIdNamePairs(string username)
    {
        var result = new Dictionary<int, string>();
        foreach (var game in _context.SavedGames)
        {
            var gameState = System.Text.Json.JsonSerializer.Deserialize<GameState>(game.State);
            if (gameState!.XPlayerUsername == username || gameState.OPlayerUsername == username)
            {
                result[game.Id] = $"{gameState.GameConfiguration.Name} | {game.CreatedAtDateTime} | " +
                                  $"{gameState.XPlayerUsername} VS {gameState.OPlayerUsername}";
            }
        }

        return result;
        
        // return _context.SavedGames
        //     .Include(c => c.Configuration) // Ensure Configuration is eagerly loaded
        //     .Where(c => c.Configuration != null)
        //     .OrderBy(c => c.CreatedAtDateTime)
        //     .ToDictionary(c => c.Id, c => c.Configuration!.Name + " | " + c.CreatedAtDateTime + " | " + c.State);
    }
}