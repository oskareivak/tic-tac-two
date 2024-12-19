using GameBrain;

namespace ConsoleUI;

public static class Visualizer
{
    public static void DrawBoard(TicTacTwoBrain gameInstance)
    {
        List<(int x, int y)> currentGridCoordinates = gameInstance.CurrentGridCoordinates
            .Select(coord => (coord[0], coord[1]))
            .ToList();

        List<string> guide = [];
        if (gameInstance.GetGameMode() != EGameMode.AivAi)
        {
            if (gameInstance.CanPlacePiece())
            {
                guide.Add("   | * Place a piece by writing coordinates: X,Y");
            }
            if (gameInstance.CanMovePieceOrMoveGrid())
            {   
                if (gameInstance.CanPlacePieceOutsideGrid())
                {
                    guide.Add("   |   (You can now also place a piece outside of the grid)");
                }
                guide.Add("   | * Move a piece by writing coordinates: X,Y X,Y");
                guide.Add("   | * Move the grid by writing a direction acronym: ");
                guide.Add("   |   N, S, E, W, NE, NW, SE, SW");
            }
        }
        else
        {
            guide.Add("   | * Press enter for AI to make a move.");
        }
        
        guide.Add("   | * Save the game by writing: save");
        guide.Add("   | * Exit the game by writing: exit");
        guide.Add("");
        var guideLinesPrinted = 0;
        var guideStartRow = Math.Max(0, gameInstance.DimY - guide.Count);
        
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
                
                Console.Write(" " + DrawGamePiece(gameInstance.GameBoard[x][y]) + " ");
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
            
            // Prints guide for user next to board.
            if (guideLinesPrinted < guide.Count && y >= gameInstance.DimY - guide.Count / 2)
            {
                Console.Write(guide[guideLinesPrinted]);
                guideLinesPrinted++;
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
                
                // Prints guide for user next to board.
                if (guideLinesPrinted < guide.Count && y >= gameInstance.DimY - guide.Count / 2 - 1)
                {
                    Console.Write(guide[guideLinesPrinted]);
                    guideLinesPrinted++;
                }
                
                Console.WriteLine();
            }
        }
    }
    
    public static string DrawGamePiece(EGamePiece piece)
    {
        switch (piece)
        {
            case EGamePiece.O: return "O";
            case EGamePiece.X: return "X";
        }

        return " ";
    }
}

