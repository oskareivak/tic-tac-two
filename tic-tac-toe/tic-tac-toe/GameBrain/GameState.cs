using System.Text.Json.Serialization;

namespace GameBrain;

public class GameState
{   
    public EGamePiece[][] GameBoard { get; set; }
    
    public GameConfiguration GameConfiguration { get; set; }
    
    public EGamePiece NextMoveBy { get; set; }
    

    public List<(int x, int y)> CurrentGridCoordinates { get; set; }

    public List<(int x, int y)> BoardCoordinates { get; set; }
      
    public int NumberOfMovesMade { get; set; }

    public Dictionary<EGamePiece, int> NumberOfPiecesOnBoard { get; set; }

    public GameState(EGamePiece[][] gameBoard, EGamePiece nextMoveBy, GameConfiguration gameConfiguration, List<(int x, int y)> currentGridCoordinates, List<(int x, int y)> boardCoordinates, int numberOfMovesMade, Dictionary<EGamePiece, int> numberOfPiecesOnBoard)
    {
        GameBoard = gameBoard;
        GameConfiguration = gameConfiguration;
        CurrentGridCoordinates = currentGridCoordinates;
        BoardCoordinates = boardCoordinates;
        NumberOfMovesMade = numberOfMovesMade;
        NextMoveBy = nextMoveBy;
        NumberOfPiecesOnBoard = numberOfPiecesOnBoard;
    }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}