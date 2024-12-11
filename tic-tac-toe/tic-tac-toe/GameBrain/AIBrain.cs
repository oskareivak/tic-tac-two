namespace GameBrain;

public class AIBrain
{
    // TODO: implement AI
    public void GetMove(TicTacTwoBrain gameInstance)
    {
        gameInstance.PlaceAPiece(4,4);
        Console.WriteLine(gameInstance.GameBoard);
        
    }
}