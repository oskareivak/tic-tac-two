namespace GameBrain;

public class TicTacTwoBrain
{
    private int DimensionX { get; set; } = 3;
    private int DimensionY { get; set; } = 3;

    private EGamePiece[,] _gameBoard;

    private static GameConfiguration _gameConfiguration;

    public List<(int x, int y)> CurrentGridCoordinates;

    private List<(int x, int y)> _boardCoordinates;

    public EGamePiece NextMoveBy;
      
    private int _numberOfMovesMade;
      
      
    public TicTacTwoBrain(GameConfiguration gameConfiguration)
    {
        _gameConfiguration = gameConfiguration;
        _gameBoard = new EGamePiece[_gameConfiguration.BoardSize, _gameConfiguration.BoardSize];
        NextMoveBy = _gameConfiguration.WhoStarts;
        _numberOfMovesMade = 0;
        _boardCoordinates = _getBoardCoordinates();
        CurrentGridCoordinates = _getInitialGridCoordinates();
    }
      
    public readonly Dictionary<string, (int x, int y)> DirectionMap = new()
    {
        { "n", (0, -1) }, // North
        { "s", (0, 1) },  // South
        { "e", (1, 0) },  // East
        { "w", (-1, 0) }, // West
        { "ne", (1, -1) }, // North-East
        { "nw", (-1, -1) }, // North-West
        { "se", (1, 1) }, // South-East
        { "sw", (-1, 1) }  // South-West
    };
      
    private readonly Dictionary<EGamePiece, int> _numberOfPiecesOnBoard = new()
    {
        { EGamePiece.X, 0 },
        { EGamePiece.O, 0 },
        { EGamePiece.Empty, _gameConfiguration.BoardSize * _gameConfiguration.BoardSize } 
    };

    public EGamePiece[,] GameBoard
    {
        get => GetBoard();
        private set => _gameBoard = value;
    }

    public int DimX => _gameBoard.GetLength(0);
    public int DimY => _gameBoard.GetLength(1);
      
      
    private EGamePiece[,] GetBoard()
    {
        var copyOfBoard = new EGamePiece[_gameBoard.GetLength(0), _gameBoard.GetLength(1)];
        for (var x = 0; x < _gameBoard.GetLength(0); x++)
        { 
            for (var y = 0; y < _gameBoard.GetLength(1); y++)
            {
                copyOfBoard[x, y] = _gameBoard[x, y];
            }
        }
 
        return copyOfBoard;
    }
    
      //hoida eraldi, et iga kord ei pea arvutama; et saaks ka muuta brainist. InGrid method, mis vaatab kas on gridis

    private List<(int x, int y)> _getInitialGridCoordinates()
    {   
        List<(int x, int y)> gridCoordinates = new List<(int x, int y)>();
          
        var topLeftCoord = ((_gameConfiguration.BoardSize - _gameConfiguration.GridSize) / 2, 
            (_gameConfiguration.BoardSize - _gameConfiguration.GridSize) / 2);
          
        gridCoordinates.Add(topLeftCoord);
          
        for (int x = 0; x < _gameConfiguration.GridSize; x++)
        {
            for (var y = 0; y < _gameConfiguration.GridSize; y++)
            {
                gridCoordinates.Add((topLeftCoord.Item1 + x, topLeftCoord.Item2 + y));
            }
        }
          
        return gridCoordinates;
    }

    private List<(int x, int y)> _getBoardCoordinates()
    {
        List<(int x, int y)> coordinates = new();
        for (int x = 0; x < _gameBoard.GetLength(0); x++)
        {
            for (int y = 0; y < _gameBoard.GetLength(1); y++)
            {
                coordinates.Add((x, y));
            }
        }
        return coordinates;
    }

    public void PlaceAPiece(int x, int y)
    {
        if (_gameBoard[x, y] != EGamePiece.Empty)
        {       
            Console.WriteLine("\nYou can't place your piece on top of another piece!");
            return;
        }
        
        if (!(_numberOfMovesMade / 2 >= _gameConfiguration.MovePieceAfterNMoves) &&
            !CurrentGridCoordinates.Contains((x, y)))
        {   
            var movesNeeded = _gameConfiguration.MovePieceAfterNMoves * 2 - _numberOfMovesMade;
            var plural = "";
            if (movesNeeded > 1)
            {
                plural = "s";
            }
            Console.WriteLine($"\nYou have to make {movesNeeded} " +
                              $"more move{plural} to place a piece outside the grid!");
            return;            
        }
        
        if (_numberOfPiecesOnBoard.TryGetValue(NextMoveBy, out int pieceCount) &&
            pieceCount >= _gameConfiguration.NumberOfPiecesPerPlayer)
        {   
            Console.WriteLine("\nYou dont have any more pieces!");
            return;            
        }
            
        _gameBoard[x, y] = NextMoveBy;
            
        _numberOfPiecesOnBoard[NextMoveBy] += 1;
        _numberOfPiecesOnBoard[EGamePiece.Empty] -= 1;
            
        // flip the next move maker/piece
        NextMoveBy = NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
          
        _numberOfMovesMade += 1;
            
    }

    public void MoveAPiece((int x, int y) from, (int x, int y) to)
    {
        if (_gameConfiguration.MovePieceAfterNMoves == 0)
        {
            Console.WriteLine("\nYou cannot move pieces in this game configuration!");
        }
        else if (!(_numberOfMovesMade / 2 >= _gameConfiguration.MovePieceAfterNMoves))
        {
            var movesNeeded = _gameConfiguration.MovePieceAfterNMoves * 2 - _numberOfMovesMade;
            var plural = "";
            if (movesNeeded > 1)
            {
                plural = "s";
            }
            Console.WriteLine($"\nYou have to make {movesNeeded} " +
                              $"more move{plural} to move a piece!");
            return;
        }
        if (_gameBoard[from.x, from.y] == EGamePiece.Empty)
        {
            Console.WriteLine("\nWhy would you want to move an empty square?");
        }
        else if (_gameBoard[from.x, from.y] != NextMoveBy)
        {
            Console.WriteLine("\nYou cannot move your opponents piece!");
        }
        else if (_gameBoard[to.x, to.y] != EGamePiece.Empty)
        {       
            Console.WriteLine("\nYou can't place your piece on top of another piece!");
        }
        else
        {
            _gameBoard[from.x, from.y] = EGamePiece.Empty;
            _gameBoard[to.x, to.y] = NextMoveBy;
            NextMoveBy = NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
        }
    }

    public void MoveGrid(string direction)
    {
        if (_gameConfiguration.MovePieceAfterNMoves == 0)
        {
            Console.WriteLine("\nYou cannot move the grid in this game configuration!");
            return;
        }
        
        List<(int x, int y)> newCoords = new List<(int x, int y)>();
        int xShift;
        int yShift;

        if (!(_numberOfMovesMade / 2 >= _gameConfiguration.MovePieceAfterNMoves))
        {
            var movesNeeded = _gameConfiguration.MovePieceAfterNMoves * 2 - _numberOfMovesMade;
            var plural = "";
            if (movesNeeded > 1)
            {
                plural = "s";
            }
            Console.WriteLine($"\nYou have to make {movesNeeded} " +
                              $"more move{plural} to move the grid!");
        }
        else if (DirectionMap.TryGetValue(direction, out (int x, int y) move))
        {
            xShift = move.x;
            yShift = move.y;
            foreach (var coordinates in CurrentGridCoordinates)
            {
                var newCoord = (coordinates.x + xShift, coordinates.y + yShift);
                newCoords.Add(newCoord);
            }

            foreach (var coordinate in newCoords)
            {
                if (!_boardCoordinates.Contains(coordinate))
                {   
                    Console.WriteLine("\nTry to keep the grid inside of the game board, ok?");
                    return;
                }
            }
            CurrentGridCoordinates = newCoords;
            NextMoveBy = NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
        }
        else
        {
            Console.WriteLine($"\nInvalid direction: {direction}");
        }
    }
      
    public EGamePiece CheckForWin()
{
    int rows = _gameBoard.GetLength(0); // Y dimension
    int cols = _gameBoard.GetLength(1); // X dimension
    int winCondition = _gameConfiguration.WinCondition; 

    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            // Only check if the current cell is not empty
            if (_gameBoard[i, j] != EGamePiece.Empty)
            {
                EGamePiece piece = _gameBoard[i, j];

                // Check horizontal
                if (CheckDirection(i, j, 0, 1, piece, winCondition)) 
                    return piece;

                // Check vertical
                if (CheckDirection(i, j, 1, 0, piece, winCondition)) 
                    return piece;

                // Check diagonal (\ direction)
                if (CheckDirection(i, j, 1, 1, piece, winCondition)) 
                    return piece;

                // Check diagonal (/ direction)
                if (CheckDirection(i, j, 1, -1, piece, winCondition)) 
                    return piece;
            }
        }
    }

    return EGamePiece.Empty; // No winner
}
    
private bool CheckDirection(int startRow, int startCol, int rowIncrement, int colIncrement, EGamePiece piece, int winCondition)
{
    int count = 0;

    for (int k = 0; k < winCondition; k++)
    {
        int newRow = startRow + k * rowIncrement;
        int newCol = startCol + k * colIncrement;

        // Check bounds
        if (newRow >= 0 && newRow < _gameBoard.GetLength(0) && newCol >= 0 && newCol < _gameBoard.GetLength(1))
        {   
            //check if it is the right piece AND that it is inside the grid
            if (_gameBoard[newRow, newCol] == piece && CurrentGridCoordinates.Contains((newRow, newCol))) 
            {
                count++;
            }
            else
            {
                break; 
            }
            
            if (count == winCondition)
            {
                return true;
            }
        }
        else
        {
            break; 
        }
    }

    return false;
}

      
      public void ResetGame()
      {
            _gameBoard = new EGamePiece[_gameBoard.GetLength(0), _gameBoard.GetLength(1)];
            NextMoveBy = _gameConfiguration.WhoStarts;
            _numberOfMovesMade = 0;
      }
}


// board & grid eraldi