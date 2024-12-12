using ConsoleApp;
using DAL;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages;

public class Home : PageModel
{
    private readonly IConfigRepository _configRepository;

    public Home(IConfigRepository configRepository)
    {
        _configRepository = configRepository;
    }

    // Bindproperty voib ara votta, aga sel juhul peab panema OnGet sisse parameetri string userName ja
    // OnGet meetodis panna UserName = userName;
    [BindProperty(SupportsGet = true)] public string UserName { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] public string? Error { get; set; }
    
    public SelectList ConfigSelectList { get; set; } = default!;
    
    public SelectList GameModeSelectList { get; set; } = default!;
    
    [BindProperty] public string SelectedGameMode { get; set; } = default!;

    [BindProperty] public int ConfigurationId { get; set; }
    
    // [BindProperty] public string ConfigurationName { get; set; } = default!;
    
    [BindProperty] public bool IsNewGame { get; set; }
    
    public IActionResult OnGet()
    {
        if (string.IsNullOrEmpty(UserName))
        {
            return RedirectToPage("./Index", new { error = "No username provided." });
        }
        
        ViewData["UserName"] = UserName;

        var selectListData = _configRepository.GetConfigurationIdNamePairs()
            .Select(pair => new { id = pair.Key, value = pair.Value })
            .ToList();
        
        ConfigSelectList = new SelectList(selectListData, "id", "value");

        var gameModeSelectListData = Settings.GameModeStrings
            .Select(pair => new { id = pair.Key, value = pair.Value })
            .ToList();
        
        // var gameModes = Enum.GetValues(typeof(EGameMode))
        //     .Cast<EGameMode>()
        //     .Select(g => new { Value = g.ToString(), Text = g.ToString() })
        //     .ToList();

        // GameModeSelectList = new SelectList(gameModes, "Value", "Text");
        GameModeSelectList = new SelectList(gameModeSelectListData, "id", "value");
        
        return Page();
    }
    
    public IActionResult OnPost()
    {
        UserName = UserName.Trim();

        // if (!string.IsNullOrWhiteSpace(UserName))
        // {
        //     return RedirectToPage("./Gameplay", new { userName = UserName, configId = ConfigurationId , IsNewGame = true });
        // }
        
        if (!string.IsNullOrWhiteSpace(UserName))
        {
            // Handle selected game mode here
            if (Enum.TryParse<EGameMode>(SelectedGameMode, out var gameMode))
            {
                // Use gameMode in logic
                return RedirectToPage("./Gameplay", new { 
                    userName = UserName, 
                    configId = ConfigurationId, 
                    isNewGame = true, 
                    gameMode = SelectedGameMode 
                });
            }
            Error = "Invalid game mode selected.";
        }
        Error = "Please enter a username.";

        return RedirectToPage("./Home", new { error = Error });
    }
    
}