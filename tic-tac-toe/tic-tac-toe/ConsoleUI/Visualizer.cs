using GameBrain;

namespace ConsoleUI;

public static class Visualizer
{
    public static void DrawBoard(TicTacTwoBrain gameInstance)
    {
        List<(int x, int y)> currentGridCoordinates = gameInstance.CurrentGridCoordinates;
        
        Console.Write("   "); // Initial spacing before X-axis indexes
        for (var x = 0; x < gameInstance.DimX; x++)
        {
            if (x > 9)
            {
                Console.Write(" " + x); // X-axis index
            }
            else
            {
                Console.Write(" " + x + " "); // X-axis index

            }
            if (x != gameInstance.DimX - 1)
            {
                Console.Write(" "); // Adjust spacing between X indexes
            }
        }
        Console.WriteLine();
        
        Console.Write("   "); // Align with Y-indexes
        for (var x = 0; x < gameInstance.DimX; x++)
        {
            Console.Write("___"); 
            if (x != gameInstance.DimX - 1)
            {
                Console.Write("_"); // Adjust spacing
            }
        }
        Console.WriteLine(); 

        
        for (var y = 0; y < gameInstance.DimY; y++)
        {
            if (y > 9)
            {
                Console.Write(y + "|"); // visualizes y index 
            }
            else
            {
                Console.Write(y + " |"); // visualizes y index
            }
            
            for (var x = 0; x < gameInstance.DimX; x++)
            {   
                if (currentGridCoordinates.Contains((x, y)))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                
                Console.Write(" " + DrawGamePiece(gameInstance.GameBoard[x, y]) + " ");
                if (x != gameInstance.DimX - 1)
                {
                    if (currentGridCoordinates.Contains((x + 1, y)) || (currentGridCoordinates.Contains((x, y)) &&
                                                                        currentGridCoordinates.Contains((x - 1, y))))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("║");
                    }
                    else
                    {
                        Console.Write("|");
                    }
                }
                
                Console.ResetColor();
            }

            Console.WriteLine();
            
            if (y != gameInstance.DimY - 1)
            {   
                Console.Write("  |"); // visualizes index border
                for (var x = 0; x < gameInstance.DimX; x++)
                {   
                    if (currentGridCoordinates.Contains((x, y)) || currentGridCoordinates.Contains((x, y + 1)))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("===");
                    }
                    else
                    {
                        Console.Write("---");
                    }
                    
                    if (x != gameInstance.DimX - 1)
                    {   
                        if (currentGridCoordinates.Contains((x + 1, y)) || 
                            currentGridCoordinates.Contains((x, y + 1)) || 
                            currentGridCoordinates.Contains((x + 1, y + 1)))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                        Console.Write("+");
                    }
                    
                    Console.ResetColor();
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

