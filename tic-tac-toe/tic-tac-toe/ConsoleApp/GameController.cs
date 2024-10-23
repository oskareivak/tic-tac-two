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
                Console.WriteLine("\nExiting...");
                return "E";
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
                Console.WriteLine("X has won the game!");
                break;
            }
            if (winner == EGamePiece.O)
            {   
                ConsoleUI.Visualizer.DrawBoard(gameInstance);
                Console.WriteLine("O has won the game!");
                break;
            }

        } while (true);
        
        // valideeri et seal on yldse koma, siis et on ainult yks koma(kas seda vaja?), split string and TRY parse, ja siis
        // validate coordinates that they actually fit on the board, is the piece there that you actually can make a move vms
        
        return "M";
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
    
    public static string NewConfiguration()
    {
        string name = "";
        int boardSize = 0;
        int gridSize = 0;
        int winCondition = 0;
        EGamePiece whoStarts = EGamePiece.Empty;
        int movePieceAfterNMoves = 10000;
        int numberOfPiecesPerPlayer = 0;
        
        do
        {
            if (name == "")
            {
                Console.WriteLine("Enter a name for your new configuration:");
                var nameInput = Console.ReadLine();
                var existingConfigNames = ConfigRepository.GetConfigurationNames();
                if (string.IsNullOrEmpty(nameInput))
                {
                    Console.WriteLine("New configuration's name can't be empty!");
                    continue;
                }
                
                if (existingConfigNames.Contains(nameInput))
                {
                    Console.WriteLine("Configuration name is already taken!");
                    continue;
                }
                
                name = nameInput;
            }

            if (boardSize == 0)
            {
                string rule = "Board side length must be between 3-40";
                Console.WriteLine("Enter board side length:");
                Console.WriteLine("(" + rule + ")");
                var boardSizeInput = Console.ReadLine();
                int boardSizeInputInt;
                try
                {
                    boardSizeInputInt = int.Parse(boardSizeInput);
                }
                catch
                {
                    Console.WriteLine("Please enter a number...");
                    continue;
                }
                
                if (boardSizeInputInt is < 3 or > 40)
                {
                    Console.WriteLine(rule);
                    continue;
                }
                
                boardSize = boardSizeInputInt;
            }

            if (gridSize == 0)
            {
                string rule = "Grid side length must be between 3-40";
                Console.WriteLine("Enter grid side length:\n");
                Console.WriteLine("(" + rule + ")");
                
                var gridSizeInput = Console.ReadLine();
                int gridSizeInputInt;
                try
                {
                    gridSizeInputInt = int.Parse(gridSizeInput);
                }
                catch
                {
                    Console.WriteLine("Please enter a number...");
                    continue;
                }
                
                if (gridSizeInputInt is < 3 or > 40)
                {
                    Console.WriteLine(rule);
                    continue;
                }
                
                gridSize = gridSizeInputInt;
            }

            if (winCondition == 0)
            {
                string rule = $"Winning condition must be between 3-{gridSize}";
                Console.WriteLine("Enter the number of pieces needed in a row to win:");
                Console.WriteLine("(" + rule + ")");
                
                var winConditionInput = Console.ReadLine();
                int winConditionInputInt;
                try
                {
                    winConditionInputInt = int.Parse(winConditionInput);
                }
                catch
                {
                    Console.WriteLine("Please enter a number...");
                    continue;
                }
                
                if (winConditionInputInt < 3 || winConditionInputInt > gridSize)
                {
                    Console.WriteLine(rule);
                    continue;
                }
                
                winCondition = winConditionInputInt;
            }

            if (whoStarts == EGamePiece.Empty)
            {
                string rule = "Enter either x or o";
                Console.WriteLine("Enter the default piece who starts the game:");
                Console.WriteLine("(" + rule + ")");
                
                var starterInput = Console.ReadLine();
                if (string.IsNullOrEmpty(starterInput) || starterInput.ToLower() != "x" && starterInput.ToLower() != "o")
                {
                    Console.WriteLine(rule);
                    continue;
                }

                if (starterInput.ToLower() == "x")
                {
                    whoStarts = EGamePiece.X;
                }
                else
                {
                    whoStarts = EGamePiece.O;
                }
            }

            if (movePieceAfterNMoves == 10000)
            {
                string rule = "Enter a reasonable number.";
                Console.WriteLine("Enter the number of moves that have to be made before you can place pieces outside" +
                                    " of the grid, move the grid and move pieces");
                Console.WriteLine("(" + rule + ")");
                
                var movePiecesAfterInput = Console.ReadLine();
                int movePiecesAfterInputInt;
                try
                {
                    movePiecesAfterInputInt = int.Parse(movePiecesAfterInput);
                }
                catch
                {
                    Console.WriteLine(rule);
                    continue;
                }
                
                if (movePiecesAfterInputInt > 170)
                {
                    Console.WriteLine(rule);
                    continue;
                }
                
                movePieceAfterNMoves = movePiecesAfterInputInt;
            }
            
            if (numberOfPiecesPerPlayer == 0)
            {
                string rule = $"Number must be between {winCondition}-80";
                Console.WriteLine("Enter number of pieces per player:");
                Console.WriteLine("(" + rule + ")");
                var piecesPerPlayerInput = Console.ReadLine();
                int piecesPerPlayerInputInt;
                try
                {
                    piecesPerPlayerInputInt = int.Parse(piecesPerPlayerInput);
                }
                catch
                {
                    Console.WriteLine("Please enter a number...");
                    continue;
                }
                
                if (piecesPerPlayerInputInt < winCondition || piecesPerPlayerInputInt > 80)
                {
                    Console.WriteLine(rule);
                    continue;
                }
                
                numberOfPiecesPerPlayer = piecesPerPlayerInputInt;
            }

            ConfigRepository.AddConfiguration(name, boardSize, gridSize, winCondition, whoStarts, 
                                                movePieceAfterNMoves, numberOfPiecesPerPlayer);
            MainLoop();

            return "";
            // return new GameConfiguration()
            // {
            //     Name = name,
            //     BoardSize = boardSize,
            //     GridSize = gridSize,
            //     WinCondition = winCondition,
            //     WhoStarts = whoStarts,
            //     MovePieceAfterNMoves = movePieceAfterNMoves,
            //     NumberOfPiecesPerPlayer = numberOfPiecesPerPlayer
            // };

        } while (true);

        
    }
}