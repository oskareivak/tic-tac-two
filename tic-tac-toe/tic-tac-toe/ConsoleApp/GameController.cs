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

    public static string MainLoop(GameState? gameState = null, string? gameStateName = null)
    {
        TicTacTwoBrain gameEngine;
        if (gameState != null)
        {
            Console.WriteLine($"xplayersname is {gameState.XPlayerUsername}, oplayersname is {gameState.OPlayerUsername}");
            var newPlayersName = string.Empty;
            if (gameState.XPlayerUsername == "....")
            {
                do
                {
                    Console.WriteLine("Please enter username for X player.");
                    var input = Console.ReadLine();
                    if (!string.IsNullOrEmpty(input) && input.ToLower() != "ai" && input != "....")
                    {
                        newPlayersName = input;
                    }
                } while (newPlayersName == string.Empty);
                
                gameState.XPlayerUsername = newPlayersName;
            }

            if (gameState.OPlayerUsername == "....")
            {
                do
                {
                    Console.WriteLine("Please enter username for O player.");
                    var input = Console.ReadLine();
                    if (!string.IsNullOrEmpty(input) && input.ToLower() != "ai" && input != "....")
                    {
                        newPlayersName = input;
                    }
                } while (newPlayersName == string.Empty);

                gameState.OPlayerUsername = newPlayersName;
            }

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

            var chosenGameModeShortcut = OptionsController.ChooseGamemode();
            if (chosenGameModeShortcut == "R")
            {
                return "R";
            }

            if (chosenGameModeShortcut == "E")
            {
                return "E";
            }

            var chosenGameMode = Enum.Parse<EGameMode>(chosenGameModeShortcut);

            var xPlayerUsername = string.Empty;
            var oPlayerUsername = string.Empty;

            var aiPiece = EGamePiece.Empty;
            if (chosenGameMode == EGameMode.PvAi)
            {
                do
                {
                    Console.WriteLine("Do you want to play as X or O?");
                    var input = Console.ReadLine();
                    if (input != null && input.ToLower() == "x")
                    {
                        aiPiece = EGamePiece.O;
                        oPlayerUsername = "AI";
                        xPlayerUsername = UserSession.Username;
                    }

                    if (input != null && input.ToLower() == "o")
                    {
                        aiPiece = EGamePiece.X;
                        xPlayerUsername = "AI";
                        oPlayerUsername = UserSession.Username;
                    }
                } while (aiPiece == EGamePiece.Empty);
            }

            if (chosenGameMode == EGameMode.AivAi)
            {
                xPlayerUsername = "AI1";
                oPlayerUsername = "AI2";
            }


            if (chosenGameMode == EGameMode.PvP)
            {
                do
                {
                    do
                    {
                        Console.WriteLine("Enter X player's username:");
                        var input = Console.ReadLine();
                        if (!string.IsNullOrEmpty(input) && input.ToLower() != "ai" && input != "....")
                        {
                            xPlayerUsername = input;
                        }
                    } while (string.IsNullOrEmpty(xPlayerUsername));

                    do
                    {
                        Console.WriteLine("Enter O player's username:");
                        var input = Console.ReadLine();
                        if (!string.IsNullOrEmpty(input) && input.ToLower() != "ai" && input != "....")
                        {
                            oPlayerUsername = input;
                        }
                    } while (string.IsNullOrEmpty(oPlayerUsername));

                    if (xPlayerUsername != UserSession.Username && oPlayerUsername != UserSession.Username)
                    {
                        Console.WriteLine($"You are logged in as {UserSession.Username}, so " +
                                          $"one player must be {UserSession.Username}.");
                    }
                } while (xPlayerUsername != UserSession.Username && oPlayerUsername != UserSession.Username);
            }

            gameEngine = new TicTacTwoBrain(chosenConfig, chosenGameMode, aiPiece, xPlayerUsername, oPlayerUsername);
            // gameEngine = new TicTacTwoBrain(chosenConfig, Enum.Parse<EGameMode>(gameMode));
        }

        var gameStateGameMode = gameEngine.GetGameMode();
        // var aiPiece = EGamePiece.Empty;
        if (gameStateGameMode == EGameMode.PvP)
        {
            Console.WriteLine("You're playing against another player.");
        }
        else if (gameStateGameMode == EGameMode.PvAi)
        {
            Console.WriteLine("\nYou're playing against AI.");
        }
        else if (gameStateGameMode == EGameMode.AivAi)
        {
            Console.WriteLine("AI is playing against AI.");
        }

        do
        {
            Console.WriteLine();
            ConsoleUI.Visualizer.DrawBoard(gameEngine);

            var aiTurn = false;
            var skip = false;

            if (gameStateGameMode == EGameMode.PvAi && gameEngine.NextMoveBy == gameEngine.GetGameState().AiPiece
                || gameStateGameMode == EGameMode.AivAi)
            {
                aiTurn = true;
            }

            Console.Write($"It's {gameEngine.WhoseTurn()}'s turn. ({gameEngine.NextMoveBy})\n");
            Console.Write(">");
            var input = "";
            if (!aiTurn)
            {
                input = Console.ReadLine()!;
            }

            if (!aiTurn)
            {
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
                            Console.WriteLine(
                                "You have reached the maximum number of saved games (100). Please delete some games before saving new ones.");
                        }
                    }
                }


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
                    var message = gameEngine.MoveGrid(input.ToLower());
                    if (message != "")
                    {
                        Console.WriteLine("\n" + message);
                    }

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

                                var message = gameEngine.MoveAPiece((fromX, fromY), (toX, toY));
                                if (message != "")
                                {
                                    Console.WriteLine("\n" + message);
                                }

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

                        var message = gameEngine.PlaceAPiece(inputX, inputY);
                        if (message != "")
                        {
                            Console.WriteLine("\n" + message);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("\nPlease write the coordinates according to the formula below. " +
                                          "\nPlease make sure that your given coordinates actually fit on the board.");
                    }
                }
            }

            if (aiTurn)
            {
                AiBrain AI = new AiBrain(gameEngine, gameEngine.GetGameState());
                var move = AI.GetMove();
                if (move.MoveType == EMoveType.PlaceAPiece)
                {
                    var message = gameEngine.PlaceAPiece(move.ToX, move.ToY);
                    if (message != "")
                    {
                        Console.WriteLine("\n" + message);
                    }
                }

                if (move.MoveType == EMoveType.MoveAPiece)
                {
                    gameEngine.MoveAPiece((move.FromX, move.FromY), (move.ToX, move.ToY));
                }

                if (move.MoveType == EMoveType.MoveGrid)
                {
                    gameEngine.MoveGrid(move.Direction);
                }

                var random = new Random();
                var delay = random.Next(Settings.AiDelayMin, Settings.AiDelayMax); // Delay for AI
                Thread.Sleep(delay);
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
                Console.WriteLine("It's a draw!");
                break;
            }

            if (winner == EGamePiece.X)
            {
                ConsoleUI.Visualizer.DrawBoard(gameEngine);
                Console.WriteLine($"{gameEngine.GetGameState().XPlayerUsername} (X) has won the game!");
                break;
            }

            if (winner == EGamePiece.O)
            {
                ConsoleUI.Visualizer.DrawBoard(gameEngine);
                Console.WriteLine($"{gameEngine.GetGameState().OPlayerUsername} (O) has won the game!");
                break;
            }

            if (winner == EGamePiece
                    .Empty) // TODO: add this to web as well? Actually probably add to CheckForWin method.
            {
                var counter = 0;
                var board = gameEngine.GetGameState().GameBoard;
                foreach (var coord in board)
                {
                    if (coord.Contains(EGamePiece.Empty))
                    {
                        counter++;
                    }
                }

                if (counter == 0)
                {
                    ConsoleUI.Visualizer.DrawBoard(gameEngine);
                    Console.WriteLine("It's a tie!");
                    break;
                }
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
            Console.WriteLine(
                "You have reached the maximum number of saved configurations (100). Please delete some configurations before saving new ones.");
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
                if (string.IsNullOrEmpty(starterInput) ||
                    starterInput.ToLower() != "x" && starterInput.ToLower() != "o")
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
                Console.WriteLine(
                    "\nEnter the number of moves that have to be made before you can place pieces outside" +
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