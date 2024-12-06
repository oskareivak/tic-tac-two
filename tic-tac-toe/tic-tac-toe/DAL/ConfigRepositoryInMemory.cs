using Domain;
using GameBrain;

namespace DAL;

public class ConfigRepositoryInMemory : IConfigRepository
{
    private List<GameConfiguration> _gameConfigurations = new List<GameConfiguration>()
    {
        new GameConfiguration()
        {
            Name = "Tic-Tac-Toe"
        },
        
        new GameConfiguration()
        {
            Name = "Tic-Tac-Two",
            BoardSize = 5,
            GridSize = 3,
            WinCondition = 3,
            MovePieceAfterNMoves = 2,
            NumberOfPiecesPerPlayer = 4,
        },
        
        new GameConfiguration()
        {
            Name = "Big board",
            BoardSize = 10,
            GridSize = 4,
            WinCondition = 4,
            MovePieceAfterNMoves = 3,
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
                                int movePieceAfterNMoves, int numberOfPiecesPerPlayer)
    {
        _gameConfigurations.Add(new GameConfiguration()
        {
            Name = name,
            BoardSize = boardSize,
            GridSize = gridSize,
            WinCondition = winCondition,
            WhoStarts = whoStarts,
            MovePieceAfterNMoves = movePieceAfterNMoves,
            NumberOfPiecesPerPlayer = numberOfPiecesPerPlayer
        });
    }
    
    public void DeleteConfiguration(string name)
    {
        return;
    }

    public GameConfiguration GetConfigurationById(int id)
    {
        throw new NotImplementedException();
    }
}