using GameBrain;

namespace DAL;

public class ConfigRepositoryDb : IConfigRepository
{
    public List<string> GetConfigurationNames() 
    {
        throw new System.NotImplementedException();
    }
    
    public GameConfiguration GetConfigurationByName(string name)
    {
        throw new System.NotImplementedException();
    }

    public void AddConfiguration(string name, int boardSize, int gridSize, int winCondition, 
                EGamePiece whoStarts, int movePieceAfterNMoves, int numberOfPiecesPerPlayer)
    {
        throw new System.NotImplementedException();
    }

    public void DeleteConfiguration(string name)
    {
        throw new System.NotImplementedException();
    }
}