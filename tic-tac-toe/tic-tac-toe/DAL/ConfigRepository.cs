using GameBrain;

namespace DAL;

public class ConfigRepository
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
        
        new GameConfiguration()
        {
            Name = "# Testing random",
            BoardSize = 7,
            GridSize = 4,
            WinCondition = 4,
            MovePieceAfterNMoves = 2,
            WhoStarts = EGamePiece.O,
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
}