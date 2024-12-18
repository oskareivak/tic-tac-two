using Common;
using DAL;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebApp.Pages;

public class Home : PageModel
{
    private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;

    public Home(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }

    // Bindproperty voib ara votta, aga sel juhul peab panema OnGet sisse parameetri string userName ja
    // OnGet meetodis panna UserName = userName;
    [BindProperty(SupportsGet = true)] public string UserName { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] public string? Error { get; set; }
    
    public SelectList ConfigSelectList { get; set; } = default!;
    
    public SelectList GameModeSelectList { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public string SelectedGameMode { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public int ConfigurationId { get; set; }
    
    [BindProperty(SupportsGet = true)] public bool IsNewGame { get; set; }
    
    [BindProperty] public string SelectedHumanPiece { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] public bool ChooseAiPiece { get; set; } = default!;
    
    public IActionResult OnGet(int configId, string gameMode)
    {
        if (string.IsNullOrWhiteSpace(UserName) || Settings.RestrictedUsernames.Contains(UserName.ToLower()) 
                                                || UserName.Length > Settings.MaxUsernameLength)
        {
            return RedirectToPage("./Index", new { error = "Invalid username provided." });
        }
        
        ViewData["UserName"] = UserName;
       
        
        ConfigurationId = configId;
        SelectedGameMode = gameMode;

        var selectListData = _configRepository.GetConfigIdNamePairsForUser(UserName)
            .Select(pair => new { id = pair.Key, value = pair.Value })
            .ToList();
    
        ConfigSelectList = new SelectList(selectListData, "id", "value", ConfigurationId);

        var gameModeSelectListData = Settings.GameModeStrings
            .Select(pair => new { id = pair.Key, value = pair.Value })
            .ToList();
    
        GameModeSelectList = new SelectList(gameModeSelectListData, "id", "value", SelectedGameMode);
    
        return Page();
    }
    
    public IActionResult OnPost()
    {
        UserName = UserName.Trim();
        
        if (!string.IsNullOrWhiteSpace(UserName) && !Settings.RestrictedUsernames.Contains(UserName.ToLower()) 
                                                 && UserName.Length <= Settings.MaxUsernameLength)
        {
            var maxGames = Settings.MaxSavedGamesPerUser;
            if (_gameRepository.GetGameNamesForUser(UserName).Count >= maxGames)
            {
                Error = $"You have reached the maximum number of saved games ({maxGames})." +
                        $" Please delete some before creating new ones.";
                
                return RedirectToPage("./Home", new { 
                    userName = UserName, 
                    error = Error
                });
            }
            
            var gameMode1 = Enum.Parse<EGameMode>(SelectedGameMode);
            if (gameMode1 == EGameMode.PvAi && string.IsNullOrWhiteSpace(SelectedHumanPiece) || 
                gameMode1 == EGameMode.PvP && string.IsNullOrWhiteSpace(SelectedHumanPiece) )
            {
                return RedirectToPage("./Home", new { 
                    userName = UserName, 
                    configId = ConfigurationId, 
                    isNewGame = true, 
                    gameMode = SelectedGameMode,
                    chooseAiPiece = true
                });
            }
            
            if (Enum.TryParse<EGameMode>(SelectedGameMode, out var gameMode))
            {
                if (gameMode1 == EGameMode.AivAi)
                {
                    return RedirectToPage("./Gameplay", new { 
                        userName = UserName, 
                        configId = ConfigurationId, 
                        isNewGame = true, 
                        gameMode = SelectedGameMode
                    });
                }
                return RedirectToPage("./Gameplay", new { 
                    userName = UserName, 
                    configId = ConfigurationId, 
                    isNewGame = true, 
                    gameMode = SelectedGameMode, 
                    selectedHumanPiece = SelectedHumanPiece
                });
            }
            Error = "Invalid game mode selected.";
        }
        Error = "Please enter a valid username.";

        return RedirectToPage("./Home", new { error = Error });
    }
    
}