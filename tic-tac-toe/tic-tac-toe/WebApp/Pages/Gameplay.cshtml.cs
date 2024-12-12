using DAL;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
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

    // [BindProperty(SupportsGet = true)]
    // [FromQuery(Name = "configName")]
    // public string ConfigurationName { get; set; } = default!;    
    
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
    public int GameId { get; set; } = default!;
    
    
    public IActionResult OnGet()
    {   
        if (string.IsNullOrEmpty(UserName))
        {
            return RedirectToPage("./Index", new { error = "No username provided." });
        }
        
        ViewData["UserName"] = UserName;
        
        if (IsNewGame)
        {   
            
            var config = _configRepository.GetConfigurationById(ConfigurationId);
            GameEngine = new TicTacTwoBrain(config);
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
        }

        if (!string.IsNullOrEmpty(GameOverMessage))
        {
            _gameRepository.DeleteGameById(GameId);
        }
        
        NextMoveBy = GameEngine.NextMoveBy;
        
        return Page();
    }

    public IActionResult OnPost()
    {
        
        if (GameEngine == null)
        {
            var savedGameState = _gameRepository.GetGameById(GameId);
            // var state = savedGame.State;
            GameEngine = new TicTacTwoBrain(savedGameState);
        }
        
        var skip = false;
        if (!string.IsNullOrEmpty(ArrowDirection))
        {
            GameEngine.MoveGrid(ArrowDirection);
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
            
            GameEngine.MoveAPiece((fromX, fromY), (toX, toY));
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
                    GameOverMessage = GameOverMessage, gameId = GameId // TODO: pole gameid probs vaja
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
