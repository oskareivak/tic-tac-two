using System.Text.Json.Serialization;

namespace GameBrain;

public class GameState
{   
    public EGamePiece[][] GameBoard { get; set; }
    
    public GameConfiguration GameConfiguration { get; set; }
    
    public EGamePiece NextMoveBy { get; set; }
    
    public EGameMode GameMode { get; set; }
    
    public EGamePiece AiPiece { get; set; }
    public int[][] CurrentGridCoordinates { get; set; }

    public int[][] BoardCoordinates { get; set; }
      
    public int NumberOfMovesMade { get; set; }

    public Dictionary<EGamePiece, int> NumberOfPiecesOnBoard { get; set; }
    
    public string XPlayerUsername { get; set; }
    
    public string OPlayerUsername { get; set; }
    
    // public GameState(EGamePiece[][] gameBoard, EGamePiece nextMoveBy, GameConfiguration gameConfiguration, int[][] currentGridCoordinates, int[][] boardCoordinates, int numberOfMovesMade, Dictionary<EGamePiece, int> numberOfPiecesOnBoard)

    // public GameState(EGamePiece[][] gameBoard, EGamePiece nextMoveBy, GameConfiguration gameConfiguration, int[][] currentGridCoordinates, int[][] boardCoordinates, int numberOfMovesMade, Dictionary<EGamePiece, int> numberOfPiecesOnBoard, string gameMode)
    
    [JsonConstructor]
    public GameState(EGamePiece[][] gameBoard, EGamePiece nextMoveBy, GameConfiguration gameConfiguration, 
                    int[][] currentGridCoordinates, int[][] boardCoordinates, int numberOfMovesMade, 
                    Dictionary<EGamePiece, int> numberOfPiecesOnBoard, EGameMode gameMode, EGamePiece aiPiece,
                    string xPlayerUsername, string oPlayerUsername)
    {
        GameBoard = gameBoard;
        GameConfiguration = gameConfiguration;
        CurrentGridCoordinates = currentGridCoordinates;
        BoardCoordinates = boardCoordinates;
        NumberOfMovesMade = numberOfMovesMade;
        NextMoveBy = nextMoveBy;
        NumberOfPiecesOnBoard = numberOfPiecesOnBoard;
        GameMode = gameMode;
        AiPiece = aiPiece;
        XPlayerUsername = xPlayerUsername;
        OPlayerUsername = oPlayerUsername;
    }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}