using System.Runtime.CompilerServices;
using System.Text.Json;

namespace GameBrain;

public class AiBrain
{
    private readonly TicTacTwoBrain _gameEngine;
    private readonly GameState _gameState;

    public AiBrain(TicTacTwoBrain gameEngine, GameState gameState)
    {
        _gameEngine = gameEngine;
        _gameState = gameState;
    }

    public Move GetMove()
    {
        var moveScores = EvaluateMoves();

        var opponentsMovesToBlock = MovesToBlockOpponentsWin();
        foreach (var move in opponentsMovesToBlock)
        {
            moveScores[move] = 8;
        }

        int maxScore = moveScores.Values.Max();

        var bestMoves = moveScores.Where(pair => pair.Value == maxScore)
            .Select(pair => pair.Key)
            .ToList();

        Random random = new Random();
        return bestMoves[random.Next(bestMoves.Count)];
    }

    private Dictionary<Move, int> EvaluateMoves()
    {
        Dictionary<Move, int> moveScores = new Dictionary<Move, int>();

        var possibleMoves = GetPossibleMoves(_gameState);

        foreach (var move in possibleMoves)
        {
            var tempGameState = DeepCopy(_gameState);
            var tempGameEngine = new TicTacTwoBrain(tempGameState);

            if (move.MoveType == EMoveType.PlaceAPiece)
            {
                tempGameEngine.PlaceAPiece(move.ToX, move.ToY);
            }
            else if (move.MoveType == EMoveType.MoveAPiece)
            {
                tempGameEngine.MoveAPiece((move.FromX, move.FromY), (move.ToX, move.ToY));
            }
            else if (move.MoveType == EMoveType.MoveGrid)
            {
                tempGameEngine.MoveGrid(move.Direction);
            }

            var winner = tempGameEngine.CheckForWin();

            if (winner == _gameState.NextMoveBy) // AI wins
            {
                moveScores.Add(move, 10);
            }
            else if (winner == EGamePiece.Empty) // Nobody wins
            {
                moveScores.Add(move, EvaluateFurther(move));
            }
            else if (winner == null) // Draw
            {
                moveScores.Add(move, -4);
            }
            else // Opponent wins
            {
                moveScores.Add(move, -10);
            }
        }

        return moveScores;
    }

    private int EvaluateFurther(Move move)
    {
        if (move.MoveType == EMoveType.PlaceAPiece)
        {
            if (HaveMyPieceNearNewPlacement(move))
            {
                return 6;
            }
        }
        else if (move.MoveType == EMoveType.MoveAPiece)
        {
            if (HaveMyPieceNearNewPlacement(move))
            {
                return 4;
            }
        }
        else if (move.MoveType == EMoveType.MoveGrid)
        {
            if (HaveMoreOfMyPiecesInGrid(move))
            {
                return 2;
            }
        }

        return 0;
    }

    private List<Move> MovesToBlockOpponentsWin()
    {
        var opponentsMoves = GetPossibleMoves(OpponentsTurnDeepCopy(_gameState));
        var myMoves = GetPossibleMoves(_gameState);
        var movesToBlockOpponentsWin = new List<Move>();

        foreach (var move in opponentsMoves)
        {
            var tempGameState = OpponentsTurnDeepCopy(_gameState);
            var tempGameEngine = new TicTacTwoBrain(tempGameState);

            if (move.MoveType == EMoveType.PlaceAPiece)
            {
                tempGameEngine.PlaceAPiece(move.ToX, move.ToY);
            }
            else if (move.MoveType == EMoveType.MoveAPiece)
            {
                tempGameEngine.MoveAPiece((move.FromX, move.FromY), (move.ToX, move.ToY));
            }
            else if (move.MoveType == EMoveType.MoveGrid)
            {
                tempGameEngine.MoveGrid(move.Direction);
            }

            var winner = tempGameEngine.CheckForWin();
            // Console.WriteLine(winner.ToString());
            
            if (winner == GetOpponentsPiece(_gameEngine.NextMoveBy))
            {
                if (move.MoveType == EMoveType.PlaceAPiece)
                {
                    if (myMoves.Contains(move))
                    {
                        movesToBlockOpponentsWin.Add(move);
                    }
                }
                else if (move.MoveType == EMoveType.MoveAPiece)
                {
                    var tryMove = new Move
                    {
                        MoveType = EMoveType.PlaceAPiece,
                        ToX = move.ToX,
                        ToY = move.ToY
                    };
                    if (myMoves.Contains(tryMove))
                    {
                        movesToBlockOpponentsWin.Add(tryMove);
                    }
                }
                else if (move.MoveType == EMoveType.MoveGrid)
                {
                    var newDirection = GetOppositeGridDirection(move.Direction);
                    var tryMove = new Move
                    {
                        MoveType = EMoveType.MoveGrid,
                        Direction = newDirection
                    };
                    if (myMoves.Contains(tryMove))
                        movesToBlockOpponentsWin.Add(tryMove);
                }
            }
        }

        return movesToBlockOpponentsWin;
    }

    private static string GetOppositeGridDirection(string direction)
    {
        switch (direction)
        {
            case "n":
                return "s";
            case "s":
                return "n";
            case "e":
                return "w";
            case "w":
                return "e";
            case "ne":
                return "sw";
            case "sw":
                return "ne";
            case "nw":
                return "se";
            case "se":
                return "nw";
        }

        throw new Exception("Invalid direction");
    }

    private bool HaveMyPieceNearNewPlacement(Move move)
    {
        int[][] directions = new int[][]
        {
            new int[] { -1, -1 }, new int[] { -1, 0 }, new int[] { -1, 1 },
            new int[] { 1, -1 }, new int[] { 1, 0 }, new int[] { 1, 1 },
            new int[] { 0, -1 }, new int[] { 0, 1 }
        };

        foreach (var direction in directions)
        {
            int newX = move.ToX + direction[0];
            int newY = move.ToY + direction[1];

            if (newX >= 0 && newX < _gameState.GameBoard.Length &&
                newY >= 0 && newY < _gameState.GameBoard[0].Length &&
                _gameState.GameBoard[newX][newY] == _gameState.NextMoveBy)
            {
                return true;
            }
        }

        return false;
    }

    private bool HaveMoreOfMyPiecesInGrid(Move move)
    {
        var currentNumberOfMyPiecesInGrid = 0;
        var newNumberOfMyPiecesInGrid = 0;


        foreach (var coord in _gameState.CurrentGridCoordinates)
        {
            if (_gameState.GameBoard[coord[0]][coord[1]] == _gameState.NextMoveBy)
            {
                currentNumberOfMyPiecesInGrid++;
            }
        }

        var tempGameState = DeepCopy(_gameState);
        var tempGameEngine = new TicTacTwoBrain(tempGameState);
        tempGameEngine.MoveGrid(move.Direction);

        foreach (var coord in tempGameState.CurrentGridCoordinates)
        {
            if (tempGameState.GameBoard[coord[0]][coord[1]] == tempGameState.NextMoveBy)
            {
                newNumberOfMyPiecesInGrid++;
            }
        }

        if (newNumberOfMyPiecesInGrid > currentNumberOfMyPiecesInGrid)
        {
            return true;
        }

        return false;
    }

    private List<Move> GetPossibleMoves(GameState gameState)
    {
        var boardCoords = gameState.BoardCoordinates;
        List<Move> moves = [];
        var gameEngine = new TicTacTwoBrain(gameState);

        // Add piece placing moves to list.
        if (gameEngine.CanPlacePiece())
        {
            foreach (var coord in boardCoords)
            {
                if (TryPlacePiece(coord[0], coord[1], gameState))
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

        if (gameEngine.CanMovePieceOrMoveGrid())
        {
            // Get my piece locations.
            List<int[]> myPieces = new List<int[]>();
            foreach (var coord in boardCoords)
            {
                if (gameState.GameBoard[coord[0]][coord[1]] == gameState.NextMoveBy)
                {
                    myPieces.Add(coord);
                }
            }

            // Add piece moving moves to list.
            foreach (var myPiece in myPieces)
            {
                foreach (var coord in boardCoords)
                {
                    if (TryMovePiece((myPiece[0], myPiece[1]), (coord[0], coord[1]), gameState))
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
            foreach (var direction in gameEngine.DirectionMap.Keys)
            {
                if (TryMoveGrid(direction, gameEngine, gameState))
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

    private bool TryPlacePiece(int x, int y, GameState gameState)
    {
        if (gameState.GameBoard[x][y] != EGamePiece.Empty)
        {
            return false;
        }

        if (!(gameState.NumberOfMovesMade / 2 >= gameState.GameConfiguration.MovePieceAfterNMoves) &&
            !gameState.CurrentGridCoordinates.Any(coord => coord[0] == x && coord[1] == y))
        {
            return false;
        }

        return true;
    }

    private bool TryMovePiece((int x, int y) from, (int x, int y) to, GameState gameState)
    {
        if (gameState.GameBoard[from.x][from.y] == EGamePiece.Empty)
        {
            return false;
        }

        if (gameState.GameBoard[from.x][from.y] != gameState.NextMoveBy)
        {
            return false;
        }

        if (gameState.GameBoard[to.x][to.y] != EGamePiece.Empty)
        {
            return false;
        }

        return true;
    }

    private bool TryMoveGrid(string direction, TicTacTwoBrain gameEngine, GameState gameState)
    {
        var newCoords = new List<int[]>();

        if (gameEngine.DirectionMap.TryGetValue(direction, out (int x, int y) move))
        {
            foreach (var coordinates in _gameState.CurrentGridCoordinates)
            {
                var newCoord = new int[] { coordinates[0] + move.x, coordinates[1] + move.y };
                newCoords.Add(newCoord);
            }

            foreach (var coordinate in newCoords)
            {
                if (!gameState.BoardCoordinates.Any(coord => coord[0] == coordinate[0] && coord[1] == coordinate[1]))
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    private GameState? DeepCopy(GameState original)
    {
        string json = JsonSerializer.Serialize(original);
        return JsonSerializer.Deserialize<GameState>(json);
    }

    private GameState? OpponentsTurnDeepCopy(GameState original)
    {
        string json = JsonSerializer.Serialize(original);
        var gameState = JsonSerializer.Deserialize<GameState>(json);
        gameState!.NextMoveBy = GetOpponentsPiece(original.NextMoveBy);
        return gameState;
    }

    private EGamePiece GetOpponentsPiece(EGamePiece piece)
    {
        if (piece == EGamePiece.X)
        {
            return EGamePiece.O;
        }

        return EGamePiece.X;
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
    
    public override bool Equals(object? obj)
    {
        if (obj is not Move other) return false;

        return MoveType == other.MoveType &&
               FromX == other.FromX &&
               FromY == other.FromY &&
               ToX == other.ToX &&
               ToY == other.ToY &&
               Direction == other.Direction;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(MoveType, FromX, FromY, ToX, ToY, Direction);
    }
}