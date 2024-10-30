namespace GameBrain;

public class TicTacTwoBrain
{
    private int DimensionX { get; set; } = 3;
    private int DimensionY { get; set; } = 3;

    private readonly GameState _gameState;
      
      
    public TicTacTwoBrain(GameConfiguration gameConfiguration)
    {
        // _gameConfiguration = gameConfiguration;
        // _gameBoard = new EGamePiece[_gameConfiguration.BoardSize, _gameConfiguration.BoardSize];
        // NextMoveBy = _gameConfiguration.WhoStarts;
        // _numberOfMovesMade = 0;
        // _boardCoordinates = _getBoardCoordinates();
        // CurrentGridCoordinates = _getInitialGridCoordinates();

        _numberOfPiecesOnBoard = new Dictionary<EGamePiece, int>
        {
            { EGamePiece.X, 0 },
            { EGamePiece.O, 0 },
            { EGamePiece.Empty, gameConfiguration.BoardSize * gameConfiguration.BoardSize } 
        };
        
        var initialGridCoordinates = _getInitialGridCoordinates(gameConfiguration);
        var boardCoordinates = _getBoardCoordinates(gameConfiguration.BoardSize);

        var gameBoard = new EGamePiece[gameConfiguration.BoardSize][];
        for (var x = 0; x < gameBoard.Length; x++)
        {
            gameBoard[x] = new EGamePiece[gameConfiguration.BoardSize];
        }
        
        _gameState = new GameState(gameBoard, gameConfiguration.WhoStarts, gameConfiguration, 
            initialGridCoordinates, boardCoordinates, 0, _numberOfPiecesOnBoard);
    }

    public string GetGameStateJson()
    {
        return _gameState.ToString();
    }

    public string GetGameConfigName()
    {
        return _gameState.GameConfiguration.Name;
    }
      
    public EGamePiece NextMoveBy => _gameState.NextMoveBy;
    public List<(int x, int y)> CurrentGridCoordinates => _gameState.CurrentGridCoordinates;
    
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
      
    private readonly Dictionary<EGamePiece, int> _numberOfPiecesOnBoard;

    public EGamePiece[][] GameBoard
    {
        get => GetBoard();
        private set => _gameState.GameBoard = value;
    }

    public int DimX => _gameState.GameBoard.Length;
    public int DimY => _gameState.GameBoard[0].Length;
      
      
    private EGamePiece[][] GetBoard()
    {
        var copyOfBoard = new EGamePiece[_gameState.GameBoard.GetLength(0)][];
            //, _gameState.GameBoard.GetLength(1)];
        for (var x = 0; x < _gameState.GameBoard.Length; x++)
        {   
            copyOfBoard[x] = new EGamePiece[_gameState.GameBoard[x].Length];
            for (var y = 0; y < _gameState.GameBoard.Length; y++)
            {
                copyOfBoard[x][y] = _gameState.GameBoard[x][y];
            }
        }
 
        return copyOfBoard;
    }
    
    private List<(int x, int y)> _getInitialGridCoordinates(GameConfiguration gameConfiguration)
    {   
        List<(int x, int y)> gridCoordinates = new List<(int x, int y)>();
        
        var topLeftCoord = ((gameConfiguration.BoardSize - gameConfiguration.GridSize) / 2, 
            (gameConfiguration.BoardSize - gameConfiguration.GridSize) / 2);
          
        gridCoordinates.Add(topLeftCoord);
          
        for (int x = 0; x < gameConfiguration.GridSize; x++)
        {
            for (var y = 0; y < gameConfiguration.GridSize; y++)
            {
                gridCoordinates.Add((topLeftCoord.Item1 + x, topLeftCoord.Item2 + y));
            }
        }
          
        return gridCoordinates;
    }

    private List<(int x, int y)> _getBoardCoordinates(int boardSize)
    {
        List<(int x, int y)> coordinates = new();
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                coordinates.Add((x, y));
            }
        }
        return coordinates;
    }

    public void PlaceAPiece(int x, int y)
    {
        if (_gameState.GameBoard[x][y] != EGamePiece.Empty)
        {       
            Console.WriteLine("\nYou can't place your piece on top of another piece!");
            return;
        }
        
        if (!(_gameState.NumberOfMovesMade / 2 >= _gameState.GameConfiguration.MovePieceAfterNMoves) &&
            !_gameState.CurrentGridCoordinates.Contains((x, y)))
        {   
            var movesNeeded = _gameState.GameConfiguration.MovePieceAfterNMoves * 2 - _gameState.NumberOfMovesMade;
            var plural = "";
            if (movesNeeded > 1)
            {
                plural = "s";
            }
            Console.WriteLine($"\nYou have to make {movesNeeded} " +
                              $"more move{plural} to place a piece outside the grid!");
            return;            
        }
        
        if (_numberOfPiecesOnBoard.TryGetValue(_gameState.NextMoveBy, out int pieceCount) &&
            pieceCount >= _gameState.GameConfiguration.NumberOfPiecesPerPlayer)
        {   
            Console.WriteLine("\nYou dont have any more pieces!");
            return;            
        }
            
        _gameState.GameBoard[x][y] = _gameState.NextMoveBy;
            
        _numberOfPiecesOnBoard[_gameState.NextMoveBy] += 1;
        _numberOfPiecesOnBoard[EGamePiece.Empty] -= 1;
            
        // flip the next move maker/piece
        _gameState.NextMoveBy = _gameState.NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
          
        _gameState.NumberOfMovesMade += 1;
            
    }

    public void MoveAPiece((int x, int y) from, (int x, int y) to)
    {
        if (_gameState.GameConfiguration.MovePieceAfterNMoves == 0)
        {
            Console.WriteLine("\nYou cannot move pieces in this game configuration!");
        }
        else if (!(_gameState.NumberOfMovesMade / 2 >= _gameState.GameConfiguration.MovePieceAfterNMoves))
        {
            var movesNeeded = _gameState.GameConfiguration.MovePieceAfterNMoves * 2 - _gameState.NumberOfMovesMade;
            var plural = "";
            if (movesNeeded > 1)
            {
                plural = "s";
            }
            Console.WriteLine($"\nYou have to make {movesNeeded} " +
                              $"more move{plural} to move a piece!");
            return;
        }
        if (_gameState.GameBoard[from.x][from.y] == EGamePiece.Empty)
        {
            Console.WriteLine("\nWhy would you want to move an empty square?");
        }
        else if (_gameState.GameBoard[from.x][from.y] != _gameState.NextMoveBy)
        {
            Console.WriteLine("\nYou cannot move your opponents piece!");
        }
        else if (_gameState.GameBoard[to.x][to.y] != EGamePiece.Empty)
        {       
            Console.WriteLine("\nYou can't place your piece on top of another piece!");
        }
        else
        {
            _gameState.GameBoard[from.x][from.y] = EGamePiece.Empty;
            _gameState.GameBoard[to.x][to.y] = _gameState.NextMoveBy;
            _gameState.NextMoveBy = _gameState.NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
        }
    }

    public void MoveGrid(string direction)
    {
        if (_gameState.GameConfiguration.MovePieceAfterNMoves == 0)
        {
            Console.WriteLine("\nYou cannot move the grid in this game configuration!");
            return;
        }
        
        List<(int x, int y)> newCoords = new List<(int x, int y)>();
        int xShift;
        int yShift;

        if (!(_gameState.NumberOfMovesMade / 2 >= _gameState.GameConfiguration.MovePieceAfterNMoves))
        {
            var movesNeeded = _gameState.GameConfiguration.MovePieceAfterNMoves * 2 - _gameState.NumberOfMovesMade;
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
            foreach (var coordinates in _gameState.CurrentGridCoordinates)
            {
                var newCoord = (coordinates.x + xShift, coordinates.y + yShift);
                newCoords.Add(newCoord);
            }

            foreach (var coordinate in newCoords)
            {
                if (!_gameState.BoardCoordinates.Contains(coordinate))
                {   
                    Console.WriteLine("\nTry to keep the grid inside of the game board, ok?");
                    return;
                }
            }
            _gameState.CurrentGridCoordinates = newCoords;
            _gameState.NextMoveBy = _gameState.NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
        }
        else
        {
            Console.WriteLine($"\nInvalid direction: {direction}");
        }
    }
      
    public EGamePiece? CheckForWin()
{
    int rows = _gameState.GameBoard.Length; // Y dimension
    int cols = _gameState.GameBoard[0].Length; // X dimension
    int winCondition = _gameState.GameConfiguration.WinCondition; 

    bool xWins = false;
    bool oWins = false;

    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            if (_gameState.GameBoard[i][j] != EGamePiece.Empty)
            {
                EGamePiece piece = _gameState.GameBoard[i][j];

                // Check horizontal
                if (CheckDirection(i, j, 0, 1, piece, winCondition))
                {
                    if (piece == EGamePiece.X) xWins = true;
                    if (piece == EGamePiece.O) oWins = true;
                }

                // Check vertical
                if (CheckDirection(i, j, 1, 0, piece, winCondition))
                {
                    if (piece == EGamePiece.X) xWins = true;
                    if (piece == EGamePiece.O) oWins = true;
                }

                // Check diagonal (\ direction)
                if (CheckDirection(i, j, 1, 1, piece, winCondition))
                {
                    if (piece == EGamePiece.X) xWins = true;
                    if (piece == EGamePiece.O) oWins = true;
                }

                // Check diagonal (/ direction)
                if (CheckDirection(i, j, 1, -1, piece, winCondition))
                {
                    if (piece == EGamePiece.X) xWins = true;
                    if (piece == EGamePiece.O) oWins = true;
                }
            }
        }
    }
    
    if (xWins && oWins) return null; // tie
    if (xWins) return EGamePiece.X;
    if (oWins) return EGamePiece.O;

    return EGamePiece.Empty; // no winner
}
    
private bool CheckDirection(int startRow, int startCol, int rowIncrement, int colIncrement, EGamePiece piece, int winCondition)
{
    int count = 0;

    for (int k = 0; k < winCondition; k++)
    {
        int newRow = startRow + k * rowIncrement;
        int newCol = startCol + k * colIncrement;

        // check bounds
        if (newRow >= 0 && newRow < _gameState.GameBoard.Length && newCol >= 0 && newCol < _gameState.GameBoard[0].Length)
        {   
            //check if it is the right piece AND that it is inside the grid
            if (_gameState.GameBoard[newRow][newCol] == piece && _gameState.CurrentGridCoordinates.Contains((newRow, newCol))) 
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
          
          var gameBoard = new EGamePiece[_gameState.GameConfiguration.BoardSize][];
          for (var x = 0; x < gameBoard.Length; x++)
          {
              gameBoard[x] = new EGamePiece[_gameState.GameConfiguration.BoardSize];
          }
          
          _gameState.GameBoard = gameBoard;
          _gameState.NextMoveBy = _gameState.GameConfiguration.WhoStarts;
          _gameState.NumberOfMovesMade = 0;
      }
}
