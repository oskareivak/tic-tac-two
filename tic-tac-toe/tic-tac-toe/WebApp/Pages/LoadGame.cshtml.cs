using ConsoleApp;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages;

public class LoadGame : PageModel
{
    // private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;

    public LoadGame(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
        // _configRepository = configRepository;
    }

    // Bindproperty voib ara votta, aga sel juhul peab panema OnGet sisse parameetri string userName ja
    // OnGet meetodis panna UserName = userName;
    [BindProperty(SupportsGet = true)] public string UserName { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] public string? Error { get; set; }
    
    public SelectList GameSelectList { get; set; } = default!;

    [BindProperty] public int ConfigurationId { get; set; }
    
    [BindProperty] public int GameId { get; set; }
    
    public IActionResult OnGet()
    {
        if (string.IsNullOrEmpty(UserName))
        {
            return RedirectToPage("./Index", new { error = "No username provided." });
        }
        
        ViewData["UserName"] = UserName;

        
        var selectListData = _gameRepository.GetGameIdNamePairs(UserName)
            .Select(pair => new { id = pair.Key, value = pair.Value })
            .ToList();
        
        GameSelectList = new SelectList(selectListData, "id", "value");
        
        return Page();
    }
    
    public IActionResult OnPost()
    {
        UserName = UserName.Trim();

        if (!string.IsNullOrWhiteSpace(UserName))
        {
            if (_gameRepository.GetGameNamesForUser(UserName).Count == 0)
            {
                Error = "You don't have any games to load yet!";
                return RedirectToPage("./LoadGame", new { userName = UserName, error = Error});
            }
            
            return RedirectToPage("./Gameplay", new { userName = UserName, configId = ConfigurationId , IsNewGame = false , gameId = GameId });
        }

        Error = "Please enter a username.";

        return RedirectToPage("./Home", new { error = Error });
    }
    
}