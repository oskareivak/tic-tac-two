namespace GameBrain;

public record struct GameConfiguration()
{
    public string Name { get; set; } = default!;
        
    public int BoardSizeWidth { get; set; } = 3;
    public int BoardSizeHeight { get; set; } = 3;

    // how many pieces in a row to win
    public int WinCondition { get; set; } = 3;
    
    // 0 - disabled
    public int MovePieceAfterNMoves { get; set; } = 0;

    public override string ToString()
    {
        return $"Board {BoardSizeWidth} x {BoardSizeHeight}, to win: {WinCondition}, " +
               $"can move pieces after {MovePieceAfterNMoves} moves.";
    }
}