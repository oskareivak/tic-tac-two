using GameBrain;

namespace ConsoleUI;

public static class Visualizer
{
    public static void DrawBoard(TicTacTwoBrain gameInstance)
    {
        for (var y = 0; y < gameInstance.DimY; y++)
        {
            for (var x = 0; x < gameInstance.DimX; x++)
            {
                Console.Write(" " + DrawGamePiece(gameInstance.GameBoard[x, y]) + " ");
                if (x != gameInstance.DimX - 1)
                {
                    Console.Write("|");
                }

            }

            Console.WriteLine();
            if (y != gameInstance.DimY - 1)
            {
                for (var x = 0; x < gameInstance.DimX; x++)
                { 
                    Console.Write("---");
                    if (x != gameInstance.DimX - 1)
                    {
                        Console.Write("+");
                    }
                }

                Console.WriteLine();
            }
        }
    }
    
    private static string DrawGamePiece(EGamePiece piece)
    {
        switch (piece)
        {
            case EGamePiece.O: return "O";
            case EGamePiece.X: return "X";
        }

        return " ";
    }
}

