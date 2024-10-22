namespace GameBrain;

public record struct GameConfiguration()
{
    public string Name { get; set; } = default!;
        
    public int BoardSize { get; set; } = 3;

    public int GridSize { get; set; } = 3;

    // public int GridSizeHeight { get; set; } = 3;

    // how many pieces in a row to win
    public int WinCondition { get; set; } = 3;

    public EGamePiece WhoStarts { get; set; } = EGamePiece.X;
    
    // 0 - disabled
    public int MovePieceAfterNMoves { get; set; } = 0;

    public int NumberOfPiecesPerPlayer { get; set; } = 5;

    public override string ToString()
    {
        return $"Board: {BoardSize} x {BoardSize}, grid: {GridSize} x {GridSize} to win: {WinCondition}, " +
               $"can move pieces after {MovePieceAfterNMoves} moves.";
    }
}