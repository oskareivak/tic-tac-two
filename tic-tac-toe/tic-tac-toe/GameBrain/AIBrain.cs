namespace GameBrain;

public class AIBrain
{
    public void GetMove(TicTacTwoBrain gameInstance)
    {
        gameInstance.PlaceAPiece(4,4);
        Console.WriteLine(gameInstance.GameBoard);
        
    }
}