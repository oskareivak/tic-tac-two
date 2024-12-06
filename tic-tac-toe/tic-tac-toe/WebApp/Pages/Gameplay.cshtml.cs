using DAL;
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
    
    // [BindProperty(SupportsGet = true)] public int GameId { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public EGamePiece NextMoveBy
    {
        get => _nextMoveBy;
        set => _nextMoveBy = value;
    }
    
    [BindProperty(SupportsGet = true)] 
    public int ConfigurationId { get; set; }

    [BindProperty(SupportsGet = true)]
    [FromQuery(Name = "configName")]
    public string ConfigurationName { get; set; } = default!;    
    
    [BindProperty(SupportsGet = true)]
    public bool IsNewGame { get; set; } = default!;
    
    public TicTacTwoBrain GameEngine { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] 
    public string UserName { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] public string? Error { get; set; }
    
    [BindProperty]
    public string From { get; set; } = default!;

    [BindProperty]
    public string To { get; set; } = default!;

    [BindProperty(SupportsGet = true)] public string State { get; set; } = default!;
    
    public IActionResult OnGet()
    {   
        if (string.IsNullOrEmpty(UserName))
        {
            return RedirectToPage("./Index", new { error = "No username provided." });
        }
        
        ViewData["UserName"] = UserName;
        
        // Log the ConfigurationName value
        // ViewData["ConfigurationId"] = ConfigurationId;
        
        // ViewData["ConfigurationId"] = ConfigurationId;
        
        
        // var dbGame = _gameRepository.GetSavedGame(GameId);
        
        // var gameStateJson = System.Text.Json.JsonSerializer.Deserialize<GameState>(dbGame.State) 
        //                     ?? throw new Exception("Deserialization failed");

        // TicTacTwoBrain = new TicTacTwoBrain(gameStateJson);
        
        // var config = _configRepository.GetConfigurationById(ConfigurationId);
        // var config = _configRepository.GetConfigurationByName()

        if (IsNewGame)
        {
            var config = _configRepository.GetConfigurationByName(ConfigurationName);
            GameEngine = new TicTacTwoBrain(config);
        
        }
        else
        {
            if (!string.IsNullOrEmpty(State))
            {
                // TODO: vota postist sisse.
                // TicTacTwoBrain = ;
                // _gameRepository.SaveGame();
                // TicTacTwoBrain.GetGameStateJson();
                
                GameState gameState = TicTacTwoBrain.FromJson(State);
                GameEngine  = new TicTacTwoBrain(gameState);
            }
            
        }
        
        
        
        NextMoveBy = GameEngine.NextMoveBy;
        
        return Page();
    }
    
    public IActionResult OnPost()
    {
        // TODO: remove later
        Console.WriteLine($"From: {From}, To: {To}");
        
        // Ensure GameEngine is initialized
        if (GameEngine == null)
        {
            if (!string.IsNullOrEmpty(State))
            {
                GameState gameState = TicTacTwoBrain.FromJson(State);
                GameEngine = new TicTacTwoBrain(gameState);
            }
            else
            {
                var config = _configRepository.GetConfigurationByName(ConfigurationName);
                GameEngine = new TicTacTwoBrain(config);
            }
        }
        if (string.IsNullOrEmpty(From))
        {
            // place piece
            var splitTo = To.Split(',');
            var toX = int.Parse(splitTo[0]);
            var toY = int.Parse(splitTo[1]);
            
            GameEngine.PlaceAPiece(toX, toY);
        }
        else if (!string.IsNullOrEmpty(From) && !string.IsNullOrEmpty(To))
        {
            // move piece
            var splitFrom = From.Split(',');
            var splitTo = To.Split(',');
            var fromX = int.Parse(splitFrom[0]);
            var fromY = int.Parse(splitFrom[1]);
            var toX = int.Parse(splitTo[0]);
            var toY = int.Parse(splitTo[1]);
            
            GameEngine.MoveAPiece((fromX, fromY), (toX, toY));
        }
        
        UserName = UserName.Trim();
        
        // var brainJson = System.Text.Json.JsonSerializer.Serialize(TicTacTwoBrain);
        var stateJson = GameEngine.GetGameStateJson();

        if (!string.IsNullOrWhiteSpace(UserName))
        {
            return RedirectToPage("./Gameplay", new { userName = UserName, configName = ConfigurationName , State = stateJson, IsNewGame = false });
        }

        Error = "Please enter a username.";

        return RedirectToPage("./Home", new { error = Error });
    }
    
}