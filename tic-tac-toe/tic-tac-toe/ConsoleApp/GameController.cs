using DAL;
using GameBrain;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp;

public static class GameController
{
    private static readonly IConfigRepository ConfigRepository;
    private static readonly IGameRepository GameRepository;
    
    static GameController()
    {
        if (Settings.Mode == ESavingMode.Json)
        {
            ConfigRepository = new ConfigRepositoryJson();
            GameRepository = new GameRepositoryJson();
        }
        else if (Settings.Mode == ESavingMode.Database)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite($"Data Source={FileHelper.BasePath}app.db");
            var context = new AppDbContext(optionsBuilder.Options);

            ConfigRepository = new ConfigRepositoryDb(context);
            GameRepository = new GameRepositoryDb(context);
        }
        else
        {
            ConfigRepository = new ConfigRepositoryInMemory();
            GameRepository = new NoOpGameRepository();
        }
    }

    public static string MainLoop(GameState? gameState = null, string? gameStateName = null, string? chosenGameMode = null)
    {
        TicTacTwoBrain gameEngine;
        if (gameState != null)
        {
            gameEngine = new TicTacTwoBrain(gameState);
        }
        else
        {
            var chosenConfigShortcut = OptionsController.ChooseConfiguration(ConfigRepository);
    
            if (!int.TryParse(chosenConfigShortcut, out var configNo))
            {
                return chosenConfigShortcut;
            }

            var chosenConfig = ConfigRepository.GetConfigurationByName(
                ConfigRepository.GetConfigurationNames()[configNo]
            );

            // var gameMode = OptionsController.ChooseGamemode();
            
            // gameEngine = new TicTacTwoBrain(chosenConfig, chosenGameMode);
            gameEngine = new TicTacTwoBrain(chosenConfig);

        }
        
        // var gameStateGameMode = gameEngine.GetGameMode();
        // if (gameStateGameMode == "PvP")
        // {
        //     Console.WriteLine("You're playing against another player.");
        // }
        // else if (gameStateGameMode == "PvAI")
        // {
        //     Console.WriteLine("You're playing against AI.");
        // }
        // else if (gameStateGameMode == "AIvAI")
        // {
        //     Console.WriteLine("AI is playing against AI.");
        // }

        do
        {   
            Console.WriteLine();
            ConsoleUI.Visualizer.DrawBoard(gameEngine);
            
            Console.Write($"It's {gameEngine.NextMoveBy}'s turn.\n");
            // Console.Write("Give me coordinates <x,y>:");
            Console.Write(">");
            //  or save. To be added later.
            var input = Console.ReadLine()!;
            var skip = false;
            
            // Not used when using in-memory saving:
            if (Settings.Mode == ESavingMode.Json || Settings.Mode == ESavingMode.Database)
            {
                if (input.ToLower() == "save")
                {   
                    if (gameState != null && gameStateName != null)
                    {
                        GameRepository.DeleteGame(gameStateName);
                    }
                    if (GameRepository.SaveGame(gameEngine.GetGameStateJson(), gameEngine.GetGameConfigName()))
                    {
                        Console.WriteLine("Game saved!");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("You have reached the maximum number of saved games (100). Please delete some games before saving new ones.");
                    }
                    
                }
            }

            // if (gameStateGameMode == "PvAI" && gameEngine.NextMoveBy == EGamePiece.O)
            // {
            //     AIBrain aiBrain = new AIBrain();
            //     aiBrain.GetMove(gameEngine);
            //     skip = true;
            // }
            
            if (input.ToLower() == "exit")
            {   
                Console.WriteLine("\nExiting...");
                return "E";
            }
            if (input.ToLower() == "reset")
            {
                gameEngine.ResetGame();
                Console.WriteLine("\nSuccessfully reset the game!");
                continue;
            }
            if (gameEngine.DirectionMap.ContainsKey(input.ToLower()))
            {
                gameEngine.MoveGrid(input.ToLower());
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
                            gameEngine.MoveAPiece((fromX, fromY), (toX, toY));
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
                    gameEngine.PlaceAPiece(inputX, inputY);
                }
                catch (Exception)
                {   
                    Console.WriteLine("\nPlease write the coordinates according to the formula below. " +
                                      "\nPlease make sure that your given coordinates actually fit on the board.");
                }
            }
            
            //check if X or O have won the game.
            var winner = gameEngine.CheckForWin();
            
            // Not used when using in-memory saving:
            if (Settings.Mode == ESavingMode.Json || Settings.Mode == ESavingMode.Database)
            {
                if (gameState != null && gameStateName != null && winner != EGamePiece.Empty)
                {
                    GameRepository.DeleteGame(gameStateName);
                }
            }
            
            if (winner == null)
            {
                ConsoleUI.Visualizer.DrawBoard(gameEngine);
                Console.WriteLine("It's a tie!");
                break;
            }
            if (winner == EGamePiece.X)
            {   
                ConsoleUI.Visualizer.DrawBoard(gameEngine);
                Console.WriteLine("X has won the game!");
                break;
            }
            if (winner == EGamePiece.O)
            {   
                ConsoleUI.Visualizer.DrawBoard(gameEngine);
                Console.WriteLine("O has won the game!");
                break;
            }

        } while (true);
        
        return "M";
    }
    
    public static string NewConfiguration()
    {
        // var savedConfigs = Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.ConfigExtension);
        var savedConfigsCount = ConfigRepository.GetConfigurationNames().Count;
        if (savedConfigsCount >= Settings.MaxSavedConfigs)
        {
            Console.WriteLine("You have reached the maximum number of saved configurations (100). Please delete some configurations before saving new ones.");
            return "";
        }
        
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
                Console.WriteLine("\nEnter a name for your new configuration:");

                Settings.NewConfigRules.TryGetValue("gameNameLengthMin", out var rule1);
                Settings.NewConfigRules.TryGetValue("gameNameLengthMax", out var rule2);

                var nameInput = Console.ReadLine();
                var existingConfigNames = ConfigRepository.GetConfigurationNames();
                
                if (string.IsNullOrEmpty(nameInput) || nameInput.Length < rule1 || nameInput.Length > rule2)
                {
                    Console.WriteLine($"Game name must be between {rule1}-{rule2} characters.");
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
                Settings.NewConfigRules.TryGetValue("boardSideLengthMin", out var rule1);
                Settings.NewConfigRules.TryGetValue("boardSideLengthMax", out var rule2);
                
                var rule = $"Board side length must be between {rule1}-{rule2}";
                Console.WriteLine("\nEnter board side length:");
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
                
                if (boardSizeInputInt is < 3 or > 20)
                {
                    Console.WriteLine(rule);
                    continue;
                }
                
                boardSize = boardSizeInputInt;
            }

            if (gridSize == 0)
            {   
                Settings.NewConfigRules.TryGetValue("boardSideLengthMin", out var rule1);
                var rule = $"Grid side length must be between {rule1}-{boardSize}"; 
                Console.WriteLine("\nEnter grid side length:");
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
                
                if (gridSizeInputInt < rule1 || gridSizeInputInt > boardSize)
                {
                    Console.WriteLine(rule);
                    continue;
                }
                
                gridSize = gridSizeInputInt;
            }

            if (winCondition == 0)
            {   
                Settings.NewConfigRules.TryGetValue("winConditionLengthMin", out var rule1);
                var rule = $"Winning condition must be between {rule1}-{gridSize}";
                Console.WriteLine("\nEnter the number of pieces needed in a row to win:");
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
                
                if (winConditionInputInt < rule1 || winConditionInputInt > gridSize)
                {
                    Console.WriteLine(rule);
                    continue;
                }
                
                winCondition = winConditionInputInt;
            }

            if (whoStarts == EGamePiece.Empty)
            {
                var rule = "Enter either x or o";
                Console.WriteLine("\nEnter the default piece who starts the game:");
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
                Settings.NewConfigRules.TryGetValue("movePiecesAfterMin", out var rule1);
                Settings.NewConfigRules.TryGetValue("movePiecesAfterMax", out var rule2);

                
                var rule = "Enter a reasonable number.";
                Console.WriteLine("\nEnter the number of moves that have to be made before you can place pieces outside" +
                                    " of the grid, move the grid and move pieces. (0 means that they are all disabled)");
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
                
                if (movePiecesAfterInputInt > rule2 || movePiecesAfterInputInt < rule1)
                {
                    Console.WriteLine(rule);
                    continue;
                }
                
                movePieceAfterNMoves = movePiecesAfterInputInt;
            }
            
            if (numberOfPiecesPerPlayer == 0)
            {
                string rule = $"Number must be between {winCondition}-{boardSize * boardSize / 2 + 1}";
                Console.WriteLine("\nEnter number of pieces per player:");
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
                
                if (piecesPerPlayerInputInt < winCondition || piecesPerPlayerInputInt > boardSize * boardSize / 2 + 1)
                {
                    Console.WriteLine(rule);
                    continue;
                }
                
                numberOfPiecesPerPlayer = piecesPerPlayerInputInt;
            }

            ConfigRepository.AddConfiguration(name, boardSize, gridSize, winCondition, whoStarts, 
                                                movePieceAfterNMoves, numberOfPiecesPerPlayer);
            Console.WriteLine("Configuration saved!");
            // MainLoop();
            Menus.MainMenu.Run();

            return "";
        } while (true);

        
    }
}