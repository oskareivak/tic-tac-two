using GameBrain;

namespace ConsoleUI;

public static class Visualizer
{
    public static void DrawBoard(TicTacTwoBrain gameInstance)
    {   
        Console.Write("   "); // Initial spacing before X-axis indexes
        for (var x = 0; x < gameInstance.DimX; x++)
        {
            Console.Write(" " + x + " "); // X-axis index
            if (x != gameInstance.DimX - 1)
            {
                Console.Write(" "); // Adjust spacing between X indexes
            }
        }
        Console.WriteLine();
        
        Console.Write("   "); // Align with Y-indexes
        for (var x = 0; x < gameInstance.DimX; x++)
        {
            Console.Write("___"); // Print dashes to separate
            if (x != gameInstance.DimX - 1)
            {
                Console.Write("_"); // Adjust spacing
            }
        }
        Console.WriteLine(); // Move to the next line

        
        for (var y = 0; y < gameInstance.DimY; y++)
        {   
            Console.Write(y + " |"); // visualizes y index
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
                Console.Write("  |"); // visualizes index border
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

