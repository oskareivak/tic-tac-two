using ConsoleApp;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages;

public class Options : PageModel
{
// private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;
    private readonly IConfigRepository _configRepository;

    public Options(IGameRepository gameRepository, IConfigRepository configRepository)
    {
        _gameRepository = gameRepository;
        _configRepository = configRepository;
    }

    // Bindproperty voib ara votta, aga sel juhul peab panema OnGet sisse parameetri string userName ja
    // OnGet meetodis panna UserName = userName;
    [BindProperty(SupportsGet = true)] public string UserName { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] public string? Success { get; set; }
    
    [BindProperty(SupportsGet = true)] public string? Error { get; set; }
    
    public SelectList GameSelectList { get; set; } = default!;
    
    public SelectList ConfigSelectList { get; set; } = default!;

    [BindProperty] public int ConfigurationId { get; set; }
    
    [BindProperty] public int GameId { get; set; }
    
    public IActionResult OnGet()
    {
        if (string.IsNullOrEmpty(UserName))
        {
            return RedirectToPage("./Index", new { error = "No username provided." });
        }
        
        ViewData["UserName"] = UserName;

        
        var gameSelectListData = _gameRepository.GetGameIdNamePairs()
            .Select(pair => new { id = pair.Key, value = pair.Value })
            .ToList();
        
        GameSelectList = new SelectList(gameSelectListData, "id", "value");
        
        
        var configRepositoryInMemory = new ConfigRepositoryInMemory();
        var defaultConfigurations = configRepositoryInMemory.GetConfigurationNames();
        
        var configSelectListData = _configRepository.GetConfigurationIdNamePairs()
            .Where(pair => !defaultConfigurations.Contains(pair.Value))
            .Select(pair => new { id = pair.Key, value = pair.Value })
            .ToList();
        
        ConfigSelectList = new SelectList(configSelectListData, "id", "value");
        
        return Page();
    }
    
    public IActionResult OnPost()
    {
        UserName = UserName.Trim();
    
        if (!string.IsNullOrWhiteSpace(UserName))
        {
            var savedConfigsCount = _configRepository.GetConfigurationNames().Count;
            var configRepositoryInMemory = new ConfigRepositoryInMemory();
            var defaultConfigurationsCount = configRepositoryInMemory.GetConfigurationNames().Count;
            
            if (Request.Form.ContainsKey("deleteConfiguration"))
            {
                
                if (!(savedConfigsCount > defaultConfigurationsCount))
                {
                    Error = "You don't have any configurations to delete yet!";
                    return RedirectToPage("./Options", new { userName = UserName, error = Error});
                }
                
                _configRepository.DeleteConfigurationById(ConfigurationId); 
                Success = "Configuration deleted!";
                
            }
            else if (Request.Form.ContainsKey("createConfiguration"))
            {
                
                if (savedConfigsCount >= Settings.MaxSavedConfigs)
                {
                    Error = ($"You have reached the maximum number of saved configurations " +
                                      $"({savedConfigsCount-defaultConfigurationsCount}). Please delete " +
                                      $"some configurations before saving new ones.");
                    return RedirectToPage("./Options", new { userName = UserName, error = Error});
                }

                Success = "Configuration created!";
            }
            else if (Request.Form.ContainsKey("deleteGame"))
            {
                // Handle delete game
                if (_gameRepository.GetGameNames().Count == 0)
                {
                    Error = "You don't have any games to delete yet!";
                    return RedirectToPage("./Options", new { userName = UserName, error = Error});
                }
                
                _gameRepository.DeleteGameById(GameId); 
                Success = "Game deleted!";
            }
            
            return RedirectToPage("./Options", new { userName = UserName, success = Success});
        }
    
        Error = "Please enter a username.";
        
        return RedirectToPage("./Home", new { error = Error });
    }
}
