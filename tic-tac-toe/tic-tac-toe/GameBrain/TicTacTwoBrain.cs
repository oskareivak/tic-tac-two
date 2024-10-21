namespace GameBrain;

public class TicTacTwoBrain
{
      private int DimensionX { get; set; } = 3;
      private int DimensionY { get; set; } = 3;

      private EGamePiece[,] _gameBoard;

      private static GameConfiguration _gameConfiguration;

      public List<(int x, int y)> CurrentGridCoords;

      public EGamePiece NextMoveBy;
      
      private int _numberOfMovesMade;
      
      
      public TicTacTwoBrain(GameConfiguration gameConfiguration)
      {
            _gameConfiguration = gameConfiguration;
            _gameBoard = new EGamePiece[_gameConfiguration.BoardSize, _gameConfiguration.BoardSize];
            NextMoveBy = _gameConfiguration.WhoStarts;
            _numberOfMovesMade = 0;
            CurrentGridCoords = GetInitialGridCoordinates();
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
      
      public List<(int x, int y)> GetInitialGridCoordinates()
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

      public bool MakeAMove(int x, int y)
      {
            if (_gameBoard[x, y] != EGamePiece.Empty)
            {
                  return false;
            }

            _gameBoard[x, y] = NextMoveBy;
            
            // flip the next move maker/piece
            NextMoveBy = NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
            _numberOfMovesMade += 1;
                  
            return true;
      }

    public void MoveGrid(string direction)
    {
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
            Console.WriteLine($"You have to make {movesNeeded} " +
                              $"more move{plural} to move the grid!");
        }
        else if (DirectionMap.TryGetValue(direction, out (int x, int y) move))
        {
            xShift = move.x;
            yShift = move.y;
            foreach (var coordinates in CurrentGridCoords)
            {
                var newCoord = (coordinates.x + xShift, coordinates.y + yShift);
                newCoords.Add(newCoord);
            }
            
            CurrentGridCoords = newCoords;
            NextMoveBy = NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
            _numberOfMovesMade += 1;
        }
        else
        {
            Console.WriteLine($"Invalid direction: {direction}");
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
            if (_gameBoard[newRow, newCol] == piece && CurrentGridCoords.Contains((newRow, newCol))) 
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