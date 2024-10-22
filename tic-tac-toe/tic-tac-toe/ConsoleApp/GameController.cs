using DAL;
using GameBrain;
using MenuSystem;

namespace ConsoleApp;

public static class GameController
{
    private static readonly ConfigRepository ConfigRepository = new ConfigRepository();
    
    public static string MainLoop()
    {
        var chosenConfigShortcut = ChooseConfiguration();
    
        if (!int.TryParse(chosenConfigShortcut, out var configNo))
        {
            return chosenConfigShortcut;
        }

        var chosenConfig = ConfigRepository.GetConfigurationByName(
            ConfigRepository.GetConfigurationNames()[configNo]
        );
    
        var gameInstance = new TicTacTwoBrain(chosenConfig);

        
        // main loop of gameplay
        // draw the board again
        // ask input again, validate (again)
        // is the game over?

        do
        {   
            Console.WriteLine();
            ConsoleUI.Visualizer.DrawBoard(gameInstance);
            
            Console.Write($"It's {gameInstance.NextMoveBy}'s turn.\n");
            Console.Write("Give me coordinates <x,y>:");
            //  or save. To be added later.
            var input = Console.ReadLine()!;
            var skip = false;
            
            
            if (input.ToLower() == "exit")
            {   
                Console.WriteLine("\nDummy exiting...");
                continue;
            }
            if (input.ToLower() == "reset")
            {
                gameInstance.ResetGame();
                Console.WriteLine("\nSuccessfully reset the game!");
                continue;
            }
            if (gameInstance.DirectionMap.ContainsKey(input.ToLower()))
            {
                gameInstance.MoveGrid(input.ToLower());
                skip = true;
            }
            else if (!input.Contains(','))
            {
                Console.WriteLine($"\nInvalid command: {input}");
                skip = true;
            }

            if (!skip)
            {
                if (input.Contains(' '))
                {
                    var inputSplit = input.Split(" ");
                    if (inputSplit[0].Contains(',') && inputSplit[1].Contains(','))
                    {
                        try
                        {
                            var splitFrom = inputSplit[0].Split(',');
                            var splitTo = inputSplit[1].Split(',');
                            var fromX = int.Parse(splitFrom[0]);
                            var fromY = int.Parse(splitFrom[1]);
                            var toX = int.Parse(splitTo[0]);
                            var toY = int.Parse(splitTo[1]);
                            gameInstance.MoveAPiece((fromX, fromY), (toX, toY));
                            skip = true;
                        }
                        catch (Exception)
                        {
                            skip = true;
                            Console.WriteLine("\nPlease write the coordinates according to the correct formula." +
                                              "\nPlease make sure that your given coordinates actually fit on the board.");
                        }
                    }
                }
            }
            
            if (!skip)
            {   
                try
                {   
                    var inputSplit = input.Split(",");
                    var inputX = int.Parse(inputSplit[0]);
                    var inputY = int.Parse(inputSplit[1]);
                    gameInstance.PlaceAPiece(inputX, inputY);
                }
                catch (Exception)
                {   
                    Console.WriteLine("\nPlease write the coordinates according to the formula below. " +
                                      "\nPlease make sure that your given coordinates actually fit on the board.");
                }
            }
            
            //check if X or O have won the game.
            var winner = gameInstance.CheckForWin();
            if (winner == EGamePiece.X)
            {   
                ConsoleUI.Visualizer.DrawBoard(gameInstance);
                Console.WriteLine("X has won the game!\n \n");
                break;
            }
            if (winner == EGamePiece.O)
            {   
                ConsoleUI.Visualizer.DrawBoard(gameInstance);
                Console.WriteLine("O has won the game!\n \n");
                break;
            }

        } while (true);
        
        // valideeri et seal on yldse koma, siis et on ainult yks koma(kas seda vaja?), split string and TRY parse, ja siis
        // validate coordinates that they actually fit on the board, is the piece there that you actually can make a move vms
        return "Somebody won.";
    }

    private static string ChooseConfiguration()
    {
        var configMenuItems = new List<MenuItem>();

        for (int i = 0; i < ConfigRepository.GetConfigurationNames().Count; i++)
        {
            var returnValue = i.ToString();
            configMenuItems.Add(new MenuItem()
            {
                Title = ConfigRepository.GetConfigurationNames()[i],
                Shortcut = (i+1).ToString(),
                MenuItemAction = () => returnValue
            });
        }

        var configMenu = new Menu(EMenuLevel.Secondary,
            "TIC-TAC-TWO - choose game config",
            configMenuItems
            // isCustomMenu: true
        );

        return configMenu.Run(new Stack<Menu>());
    }
}