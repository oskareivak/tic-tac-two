using System.Diagnostics;
using ConsoleApp;
using DAL;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class Gameplay : PageModel
{
    private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;
    private EGamePiece _nextMoveBy = default!;

    public Gameplay(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }
    
    // TODO: add sound effects!
    
    [BindProperty(SupportsGet = true)] 
    public string? GameOverMessage { get; set; }
    
    [BindProperty]
    public string ArrowDirection { get; set; } = string.Empty;
    
    
    [BindProperty(SupportsGet = true)] 
    public EGamePiece NextMoveBy { get; set; }

    [BindProperty(SupportsGet = true)] 
    [FromQuery(Name = "configId")]
    public int ConfigurationId { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public bool IsNewGame { get; set; } = default!;
    
    public TicTacTwoBrain GameEngine { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] 
    public string UserName { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] 
    public string? Error { get; set; }
    
    [BindProperty]
    public string From { get; set; } = default!;  

    [BindProperty]
    public string To { get; set; } = default!;    

    [BindProperty(SupportsGet = true)] 
    public int GameId { get; set; } 
    
    [BindProperty(SupportsGet = true)] 
    [FromQuery(Name = "gameMode")]
    public string SelectedGameMode { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] public string SelectedHumanPiece { get; set; } = default!;
    
    public bool CanMakeMove { get; set; } = default!;
    
    
    public IActionResult OnGet()
    {   
        if (string.IsNullOrEmpty(UserName))
        {
            return RedirectToPage("./Index", new { error = "No username provided." });
        }
        
        ViewData["UserName"] = UserName;
        
        if (IsNewGame)
        {
            var aiPiece = EGamePiece.Empty;
            var xPlayerName = string.Empty;
            var oPlayerName = string.Empty;

            if (SelectedGameMode == "PvAi")
            {
                if (!string.IsNullOrEmpty(SelectedHumanPiece))
                {
                    if (SelectedHumanPiece == "X")
                    {
                        aiPiece = EGamePiece.O;
                        xPlayerName = UserName;
                        oPlayerName = "AI";
                    }
                    else
                    {
                        aiPiece = EGamePiece.X;
                        xPlayerName = "AI";
                        oPlayerName = UserName;
                    }
                }
            }
            if (SelectedGameMode == "PvP")
            {
                if (SelectedHumanPiece == "X")
                {
                    xPlayerName = UserName;
                    oPlayerName = "....";
                }
                else
                {
                    oPlayerName = UserName;
                    xPlayerName = "....";
                }
            }
            
            var config = _configRepository.GetConfigurationById(ConfigurationId);
            var gameMode = Enum.Parse<EGameMode>(SelectedGameMode);
            GameEngine = new TicTacTwoBrain(config, gameMode, aiPiece, xPlayerName, oPlayerName);
            GameId = _gameRepository.SaveGameReturnId(GameEngine.GetGameStateJson(), GameEngine.GetGameConfigName());
            return RedirectToPage("./Gameplay", new 
            { 
                userName = UserName, 
                gameId = GameId, 
                configId = ConfigurationId, 
            });
        }
        else
        {
            var savedGameState = _gameRepository.GetGameById(GameId);
            GameEngine = new TicTacTwoBrain(savedGameState);
            SelectedGameMode = GameEngine.GetGameMode().ToString();
        }

        if (!string.IsNullOrEmpty(GameOverMessage))
        {
            _gameRepository.DeleteGameById(GameId);
        }
        
        NextMoveBy = GameEngine.NextMoveBy;
        
        if (GameEngine.GetGameMode() == EGameMode.PvP && 
            NextMoveBy == EGamePiece.X && GameEngine.GetGameState().XPlayerUsername != UserName ||
            NextMoveBy == EGamePiece.O && GameEngine.GetGameState().OPlayerUsername != UserName)
        {
            CanMakeMove = false;
        }
        else
        {
            CanMakeMove = true;
        }
        Console.WriteLine(CanMakeMove);
        Console.WriteLine(NextMoveBy);
        
        return Page();
    }
    
    
    public IActionResult OnPost()
    {
        Error = "";
        
        if (GameEngine == null)
        {
            var savedGameState = _gameRepository.GetGameById(GameId);
            GameEngine = new TicTacTwoBrain(savedGameState);
        }
        
        var skip = false;
        
        var aiTurn = false;
        var gameStateGameMode = GameEngine.GetGameState().GameMode; 
        
        if (gameStateGameMode == EGameMode.PvAi && GameEngine.NextMoveBy == GameEngine.GetGameState().AiPiece 
            || gameStateGameMode == EGameMode.AivAi)
        {
            aiTurn = true;
        }

        if (!aiTurn)
        {
            if (!string.IsNullOrEmpty(ArrowDirection))
            {
                var message = GameEngine.MoveGrid(ArrowDirection);
                if (message != "")
                {
                    Error = message;
                }
            
                skip = true;
            }
        
            if (string.IsNullOrEmpty(From) && !skip)
            {
                var splitTo = To.Split(',');
                var toX = int.Parse(splitTo[0]);
                var toY = int.Parse(splitTo[1]);

                var message = GameEngine.PlaceAPiece(toX, toY);
                if (message != "")
                {
                    Error = message;
                }
            }
            else if (!string.IsNullOrEmpty(From) && !string.IsNullOrEmpty(To) && !skip)
            {
                var splitFrom = From.Split(',');
                var splitTo = To.Split(',');
                var fromX = int.Parse(splitFrom[0]);
                var fromY = int.Parse(splitFrom[1]);
                var toX = int.Parse(splitTo[0]);
                var toY = int.Parse(splitTo[1]);
            
                var message = GameEngine.MoveAPiece((fromX, fromY), (toX, toY));
                if (message != "")
                {
                    Error = message;
                }
            }
        }
        
        if (aiTurn)
        {
            AiBrain AI = new AiBrain(GameEngine, GameEngine.GetGameState());
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var move = AI.GetMove();
            stopwatch.Stop();
            Console.WriteLine("Time taken: " + stopwatch.ElapsedMilliseconds + "ms");
            if (move.MoveType == EMoveType.PlaceAPiece)
            {
                var message = GameEngine.PlaceAPiece(move.ToX, move.ToY);
                // gameEngine.PlaceAPiece(move.ToX, move.ToY);
                if (message != "")
                {
                    Console.WriteLine("\n" + message);
                }
            }

            if (move.MoveType == EMoveType.MoveAPiece)
            {
                GameEngine.MoveAPiece((move.FromX, move.FromY), (move.ToX, move.ToY));
            }

            if (move.MoveType == EMoveType.MoveGrid)
            {
                GameEngine.MoveGrid(move.Direction);
            }
            
            var random = new Random();
            var delay = random.Next(Settings.AiDelayMin, Settings.AiDelayMax); // Delay for AI
            Thread.Sleep(delay);
        }
        
        var winner = GameEngine.CheckForWin();
         
        if (winner == null)
        {
            GameOverMessage = "It's a draw!";
        }
        if (winner == EGamePiece.X)
        {
            GameOverMessage = "X has won the game!";

        }
        if (winner == EGamePiece.O)
        {   
            GameOverMessage = "O has won the game!";             
        }

        UserName = UserName.Trim();
        var stateJson = GameEngine.GetGameStateJson();
        
        if (!string.IsNullOrWhiteSpace(UserName))
        {   
            
            _gameRepository.DeleteGameById(GameId);
                
            GameId = _gameRepository.SaveGameReturnId(stateJson, GameEngine.GetGameConfigName());
            
            if (winner != EGamePiece.Empty)
            {
                return RedirectToPage("./Gameplay", new
                {
                    userName = UserName, configId = ConfigurationId, IsNewGame = false ,
                    GameOverMessage = GameOverMessage, gameId = GameId 
                });
            }
            
            return RedirectToPage("./Gameplay", new
            {
                userName = UserName, configId = ConfigurationId, IsNewGame = false, gameId = GameId, error = Error
            });
        }

        Error = "Please enter a username.";
        return RedirectToPage("./Home", new { error = Error });
    }
}
