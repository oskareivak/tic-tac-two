using Domain;
using GameBrain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class ConfigRepositoryDb : IConfigRepository
{
    private readonly AppDbContext _context;

    public ConfigRepositoryDb(AppDbContext context)
    {
        _context = context;
        CheckAndCreateInitialConfigs();
    }

    private void CheckAndCreateInitialConfigs()
    {
        if (!_context.Configurations.Any())
        {
            var hardCodedRepo = new ConfigRepositoryInMemory();
            var optionNames = hardCodedRepo.GetConfigurationNames();
            foreach (var optionName in optionNames)
            {
                var gameOption = hardCodedRepo.GetConfigurationByName(optionName);
                _context.Configurations.Add(new Configuration
                {
                    Name = gameOption.Name,
                    BoardSize = gameOption.BoardSize,
                    GridSize = gameOption.GridSize,
                    WinCondition = gameOption.WinCondition,
                    WhoStarts = (int)gameOption.WhoStarts,
                    MovePieceAfterNMoves = gameOption.MovePieceAfterNMoves,
                    NumberOfPiecesPerPlayer = gameOption.NumberOfPiecesPerPlayer
                });
            }
            _context.SaveChanges();
        }
    }

    public List<string> GetConfigurationNames()
    {
        return _context.Configurations
            .OrderBy(c => c.Name)
            .Select(c => c.Name)
            .ToList();
    }

    public GameConfiguration GetConfigurationByName(string name)
    {
        var config = _context.Configurations
            .FirstOrDefault(c => c.Name == name);

        if (config == null) throw new Exception("Configuration not found");

        return new GameConfiguration
        {
            Name = config.Name,
            BoardSize = config.BoardSize,
            GridSize = config.GridSize,
            WinCondition = config.WinCondition,
            WhoStarts = (EGamePiece)config.WhoStarts,
            MovePieceAfterNMoves = config.MovePieceAfterNMoves,
            NumberOfPiecesPerPlayer = config.NumberOfPiecesPerPlayer
        };
    }

    public void AddConfiguration(string name, int boardSize, int gridSize, int winCondition,
                EGamePiece whoStarts, int movePieceAfterNMoves, int numberOfPiecesPerPlayer)
    {
        var newConfig = new Configuration
        {
            Name = name,
            BoardSize = boardSize,
            GridSize = gridSize,
            WinCondition = winCondition,
            WhoStarts = (int)whoStarts,
            MovePieceAfterNMoves = movePieceAfterNMoves,
            NumberOfPiecesPerPlayer = numberOfPiecesPerPlayer
        };

        _context.Configurations.Add(newConfig);
        _context.SaveChanges();
    }

    public void DeleteConfiguration(string name)
    {
        var config = _context.Configurations
            .FirstOrDefault(c => c.Name == name);
        
        if (config == null) throw new Exception("Configuration not found");
        
        _context.Configurations.Remove(config);
        _context.SaveChanges();
    }
    
    public void DeleteConfigurationById(int id)
    {
        var config = _context.Configurations
            .FirstOrDefault(c => c.Id == id);

        if (config == null) throw new Exception("Configuration not found");

        _context.Configurations.Remove(config);
        _context.SaveChanges();
    }

    public GameConfiguration GetConfigurationById(int id)
    {
        // return _context.Configurations.First(c => c.Id == id);
        // return _context.SavedGames.First(g => g.Id == gameId);
        
        var config = _context.Configurations
            .FirstOrDefault(c => c.Id == id);
        
        if (config == null) throw new Exception("Configuration not found");

        return new GameConfiguration
        {
            Name = config.Name,
            BoardSize = config.BoardSize,
            GridSize = config.GridSize,
            WinCondition = config.WinCondition,
            WhoStarts = (EGamePiece)config.WhoStarts,
            MovePieceAfterNMoves = config.MovePieceAfterNMoves,
            NumberOfPiecesPerPlayer = config.NumberOfPiecesPerPlayer
        };
    }

    public Dictionary<int, string> GetConfigurationIdNamePairs()
    {
        return _context.Configurations
            .OrderBy(c => c.Name)
            .ToDictionary(c => c.Id, c => c.Name);
    }
    
    
}