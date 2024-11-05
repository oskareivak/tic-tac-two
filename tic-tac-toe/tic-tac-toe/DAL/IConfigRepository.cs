using GameBrain;

namespace DAL;

public interface IConfigRepository
{
    List<string> GetConfigurationNames();
    
    GameConfiguration GetConfigurationByName(string name);

    void AddConfiguration(string name, int boardSize, int gridSize, int winCondition, EGamePiece whoStarts,
        int movePieceAfterNMoves, int numberOfPiecesPerPlayer);

    public void DeleteConfiguration(string name);
}