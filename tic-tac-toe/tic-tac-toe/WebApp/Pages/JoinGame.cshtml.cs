using ConsoleApp;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages;

public class JoinGame : PageModel
{
    // private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;

    public JoinGame(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
        // _configRepository = configRepository;
    }

    // Bindproperty voib ara votta, aga sel juhul peab panema OnGet sisse parameetri string userName ja
    // OnGet meetodis panna UserName = userName;
    [BindProperty(SupportsGet = true)] public string UserName { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] public string? Error { get; set; }
    
    public SelectList MissingPlayerGameSelectList { get; set; } = default!;

    [BindProperty] public int ConfigurationId { get; set; }
    
    [BindProperty] public int GameId { get; set; }
    
    [BindProperty] public bool JoinedGame { get; set; }
    
    public IActionResult OnGet()
    {
        if (string.IsNullOrEmpty(UserName))
        {
            return RedirectToPage("./Index", new { error = "No username provided." });
        }
        
        ViewData["UserName"] = UserName;

        
        var selectListData = _gameRepository.GetGameIdNamePairsForUser("....")
            .Select(pair => new { id = pair.Key, value = pair.Value })
            .ToList();
        
        MissingPlayerGameSelectList = new SelectList(selectListData, "id", "value");
        
        return Page();
    }
    
    public IActionResult OnPost()
    {
        UserName = UserName.Trim();

        if (!string.IsNullOrWhiteSpace(UserName))
        {
            if (_gameRepository.GetGameNamesForUser("....").Count == 0)
            {
                Error = "There aren't any games to join right now!";
                return RedirectToPage("./JoinGame", new { userName = UserName, error = Error});
            }
            
            var maxGames = Settings.MaxSavedGamesPerUser;
            if (_gameRepository.GetGameNamesForUser(UserName).Count >= maxGames)
            {
                var game = _gameRepository.GetGameById(GameId);
                if (game.XPlayerUsername == UserName || game.OPlayerUsername == UserName)
                {
                    JoinedGame = true;
                    return RedirectToPage("./Gameplay", new { userName = UserName, configId = ConfigurationId , IsNewGame = false , gameId = GameId , joinedGame = JoinedGame });
                }
                
                Error = $"You have reached the maximum number of saved games ({maxGames})." +
                        $" Please delete some before creating new ones.";
                
                return RedirectToPage("./JoinGame", new { 
                    userName = UserName, 
                    error = Error
                });
            }
            
            JoinedGame = true;
            return RedirectToPage("./Gameplay", new { userName = UserName, configId = ConfigurationId , IsNewGame = false , gameId = GameId , joinedGame = JoinedGame });
        }

        Error = "Please enter a username.";

        return RedirectToPage("./Home", new { error = Error });
    }
    
}