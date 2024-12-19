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
        var result = new List<string>();
        foreach (var game in _context.SavedGames)
        {   
            var gameState = System.Text.Json.JsonSerializer.Deserialize<GameState>(game.State);
            if (gameState!.XPlayerUsername == username || gameState.OPlayerUsername == username)
            {   
                result.Add($"{gameState.GameConfiguration.Name} | {game.CreatedAtDateTime} | {game.Id} | " +
                           $"{gameState.XPlayerUsername} VS {gameState.OPlayerUsername}");
            }
        }

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
        try
        {
            var game = _context!.SavedGames
                .FirstOrDefault(g => g.Id == gameId);

            if (game != null)
            {
                _context.SavedGames.Attach(game); // Attach to context if not tracked
                Console.WriteLine($"Am deleting game with id [{gameId}] NOW");
                _context.SavedGames.Remove(game);
                _context.SaveChanges();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception: {e.Message}");
        }
    }
    
    public int SaveGameReturnId(string jsonStateString, string gameConfigName)
    {   
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
    
    public Dictionary<int, string> GetGameIdNamePairsForUser(string username)
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
    }

    public void UpdateGame(int gameId, string gameStateJson)
    {
        var game = _context.SavedGames.Find(gameId);

        if (game != null)
        {
            game.State = gameStateJson;
            game.CreatedAtDateTime = DateTime.Now.ToString("f");
            _context.SaveChanges();
        }
        else
        {
            Console.WriteLine($"Game not found with id: {gameId}.");
        }
    }

    public bool CanBeDeletedWeb(int gameId, string userName)
    {
        var game = _context.SavedGames.Find(gameId);

        if (string.IsNullOrEmpty(game!.CanDelete1) && string.IsNullOrEmpty(game.CanDelete2))
        {
            game.CanDelete1 = userName;
            _context.SaveChanges();
            return false;
        }

        if (!string.IsNullOrEmpty(game.CanDelete1) && string.IsNullOrEmpty(game.CanDelete2))
        {
            game.CanDelete2 = userName;
            _context.SaveChanges();
            return true;
        }
        
        if (!string.IsNullOrEmpty(game.CanDelete1) && !string.IsNullOrEmpty(game.CanDelete2))
        {
            
            return true;
        }
        
        return false;
    }
}