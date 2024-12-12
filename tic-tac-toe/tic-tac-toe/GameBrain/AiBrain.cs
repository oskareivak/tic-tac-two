namespace GameBrain;

public class AiBrain
{
    // TODO: implement AI
    // public void GetMove(TicTacTwoBrain gameInstance)
    // {
    //     gameInstance.PlaceAPiece(4,4);
    //     Console.WriteLine(gameInstance.GameBoard);
    //     
    // }

    private readonly TicTacTwoBrain _gameEngine;
    private readonly GameState _gameState;

    public AiBrain(TicTacTwoBrain gameEngine, GameState gameState)
    {
        _gameEngine = gameEngine;
        _gameState = gameState;
    }

    public List<Move> GetPossibleMoves()
    {
        var boardCoords = _gameState.BoardCoordinates;
        List<Move> moves = [];

        // Add piece placing moves to list.
        if (_gameEngine.CanPlacePiece())
        {
            foreach (var coord in boardCoords)
            {
                if (TryPlacePiece(coord[0], coord[1]))
                {
                    moves.Add(new Move
                    {
                        MoveType = EMoveType.PlaceAPiece,
                        ToX = coord[0],
                        ToY = coord[1]
                    });
                }
            }
        }

        if (_gameEngine.CanMovePieceOrMoveGrid())
        {
            // Get my piece locations.
            List<int[]> myPieces = new List<int[]>();
            foreach (var coord in boardCoords)
            {
                if (_gameState.GameBoard[coord[0]][coord[1]] == _gameState.NextMoveBy)
                {
                    myPieces.Add(coord);
                }
            }

            // Add piece moving moves to list.
            foreach (var myPiece in myPieces)
            {
                foreach (var coord in boardCoords)
                {
                    if (TryMovePiece((myPiece[0], myPiece[1]), (coord[0], coord[1])))
                    {
                        moves.Add(new Move
                        {
                            MoveType = EMoveType.MoveAPiece,
                            FromX = myPiece[0],
                            FromY = myPiece[1],
                            ToX = coord[0],
                            ToY = coord[1]
                        });
                    }
                }
            }

            // Add grid moving moves to list.
            foreach (var direction in _gameEngine.DirectionMap.Keys)
            {
                if (TryMoveGrid(direction))
                {
                    moves.Add(new Move
                    {
                        MoveType = EMoveType.MoveGrid,
                        Direction = direction
                    });
                }
            }
            
        }

        return moves;
    }

    private bool TryPlacePiece(int x, int y)
    {
        if (_gameState.GameBoard[x][y] != EGamePiece.Empty)
        {
            return false;
        }

        if (!(_gameState.NumberOfMovesMade / 2 >= _gameState.GameConfiguration.MovePieceAfterNMoves) &&
            !_gameState.CurrentGridCoordinates.Any(coord => coord[0] == x && coord[1] == y))
        {
            return false;
        }

        return true;
    }

    private bool TryMovePiece((int x, int y) from, (int x, int y) to)
    {
        if (_gameState.GameBoard[from.x][from.y] == EGamePiece.Empty)
        {
            return false;
        }

        if (_gameState.GameBoard[from.x][from.y] != _gameState.NextMoveBy)
        {
            return false;
        }

        if (_gameState.GameBoard[to.x][to.y] != EGamePiece.Empty)
        {
            return false;
        }

        return true;
    }

    public bool TryMoveGrid(string direction)
    {
        var newCoords = new List<int[]>();

        if (_gameEngine.DirectionMap.TryGetValue(direction, out (int x, int y) move))
        {
            foreach (var coordinates in _gameState.CurrentGridCoordinates)
            {
                var newCoord = new int[] { coordinates[0] + move.x, coordinates[1] + move.y };
                newCoords.Add(newCoord);
            }

            foreach (var coordinate in newCoords)
            {
                if (!_gameState.BoardCoordinates.Any(coord => coord[0] == coordinate[0] && coord[1] == coordinate[1]))
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }
}

public class Move
{
    public EMoveType MoveType { get; set; }

    public int FromX { get; set; }
    public int FromY { get; set; }

    public int ToX { get; set; }
    public int ToY { get; set; }

    public string Direction { get; set; } = default!;

    public override string ToString()
    {
        if (MoveType == EMoveType.PlaceAPiece)
        {
            return $"Place piece at [{ToX},{ToY}]";
        }
        if (MoveType == EMoveType.MoveAPiece)
        {
            return $"Move piece from [{FromX},{FromY}] to [{ToX},{ToY}]";
        }
        if (MoveType == EMoveType.MoveGrid)
        {
            return $"Move grid to [{Direction}]";
        }

        return "";
    }
}