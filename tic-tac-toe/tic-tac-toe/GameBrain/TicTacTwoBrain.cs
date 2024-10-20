namespace GameBrain;

public class TicTacTwoBrain
{
      private int DimensionX { get; set; } = 3;
      private int DimensionY { get; set; } = 3;

      private EGamePiece[,] _gameBoard;
      private EGamePiece _nextMoveBy { get; set; } = EGamePiece.X;

      private GameConfiguration _gameConfiguration;
      
      
      public TicTacTwoBrain(GameConfiguration gameConfiguration)
      {
            _gameConfiguration = gameConfiguration;
            _gameBoard = new EGamePiece[_gameConfiguration.BoardSizeWidth, _gameConfiguration.BoardSizeHeight];
      }
      

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


      public bool MakeAMove(int x, int y)
      {
            if (_gameBoard[x, y] != EGamePiece.Empty)
            {
                  return false;
            }

            _gameBoard[x, y] = _nextMoveBy;
            
            // flip the next move maker/piece
            _nextMoveBy = _nextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
                  
            return true;
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
            if (_gameBoard[newRow, newCol] == piece)
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
            _nextMoveBy = EGamePiece.X;
      }
}


// board & grid eraldi