using Domain;
using GameBrain;

namespace DAL;

public class ConfigRepositoryInMemory : IConfigRepository
{
    private readonly List<GameConfiguration> _gameConfigurations = new List<GameConfiguration>()
    {
        new GameConfiguration()
        {
            Name = "Tic-Tac-Toe",
            ConfigOwner = "GAME"
        },
        
        new GameConfiguration()
        {
            Name = "Tic-Tac-Two",
            BoardSize = 5,
            GridSize = 3,
            WinCondition = 3,
            MovePieceAfterNMoves = 2,
            NumberOfPiecesPerPlayer = 4,
            ConfigOwner = "GAME"
        },
        
        new GameConfiguration()
        {
            Name = "Big board",
            BoardSize = 10,
            GridSize = 4,
            WinCondition = 4,
            MovePieceAfterNMoves = 3,
            NumberOfPiecesPerPlayer = 16,
            ConfigOwner = "GAME"
        },
    };

    public List<string> GetConfigurationNames()
    {
        return _gameConfigurations
            .OrderBy(x => x.Name)
            .Select(config=>config.Name)
            .ToList();
    }

    public GameConfiguration GetConfigurationByName(string name)
    {
        return _gameConfigurations.Single(c => c.Name == name);
    }

    public void AddConfiguration(string name, int boardSize, int gridSize, int winCondition, EGamePiece whoStarts, 
                                int movePieceAfterNMoves, int numberOfPiecesPerPlayer, string configOwner)
    {
        throw new Exception("How did you even call this out?");
    }
    
    public void DeleteConfiguration(string name)
    {
        throw new Exception("How did you even call this out?");
        
    }

    public GameConfiguration GetConfigurationById(int id)
    {
        throw new Exception("How did you even call this out?");
    }

    public void DeleteConfigurationById(int id)
    {
        throw new Exception("How did you even call this out?");
    }

    public List<string> GetConfigNamesForUser(string username)
    {
        throw new Exception("How did you even call this out?");
    }

    public Dictionary<int, string> GetConfigIdNamePairsForUser(string username)
    {
        throw new Exception("How did you even call this out?");
    }

    public Dictionary<int, string> GetOnlyUserConfigIdNamePairsForUser(string username)
    {
        throw new Exception("How did you even call this out?");
    }
}