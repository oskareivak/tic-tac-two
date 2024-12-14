using ConsoleApp;
using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages;

public class Options : PageModel
{
    private readonly IGameRepository _gameRepository;
    private readonly IConfigRepository _configRepository;

    public Options(IGameRepository gameRepository, IConfigRepository configRepository)
    {
        _gameRepository = gameRepository;
        _configRepository = configRepository;
    }
    
    [BindProperty(SupportsGet = true)] public string UserName { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] public string? Success { get; set; }
    
    [BindProperty(SupportsGet = true)] public string? Error { get; set; }

    [BindProperty(SupportsGet = true)] public List<string> Errors { get; set; } = default!;
    
    public SelectList GameSelectList { get; set; } = default!;
    
    public SelectList ConfigSelectList { get; set; } = default!;

    [BindProperty] public int ConfigurationId { get; set; }
    
    [BindProperty] public int GameId { get; set; }
    
    // New configuration properties
    [BindProperty(SupportsGet = true)] public string NewConfigName { get; set; } = default!;
    [BindProperty(SupportsGet = true)] public int BoardSize { get; set; }
    [BindProperty(SupportsGet = true)] public int GridSize { get; set; }
    [BindProperty(SupportsGet = true)] public int WinCondition { get; set; }
    [BindProperty(SupportsGet = true)] public string WhoStarts { get; set; } = "X";
    [BindProperty(SupportsGet = true)] public int MovePiecesAfterNMoves { get; set; }
    [BindProperty(SupportsGet = true)] public int NumberOfPiecesPerPlayer { get; set; }
    
    public IActionResult OnGet()
    {
        if (string.IsNullOrEmpty(UserName))
        {
            return RedirectToPage("./Index", new { error = "No username provided." });
        }
        
        ViewData["UserName"] = UserName;

        
        var gameSelectListData = _gameRepository.GetGameIdNamePairs(UserName)
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
        Errors.Clear();
        
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
                
                Settings.NewConfigRules.TryGetValue("gameNameLengthMin", out var gameNameLengthMin);
                Settings.NewConfigRules.TryGetValue("gameNameLengthMax", out var gameNameLengthMax);
                
                if (string.IsNullOrEmpty(NewConfigName) || NewConfigName.Length < gameNameLengthMin || 
                    NewConfigName.Length > gameNameLengthMax)
                {
                    Errors.Add($"Configuration name must be between {gameNameLengthMin}-{gameNameLengthMax} " +
                               $"characters long.");
                }
                
                Settings.NewConfigRules.TryGetValue("boardSideLengthMin", out var boardSideLengthMin);
                Settings.NewConfigRules.TryGetValue("boardSideLengthMax", out var boardSideLengthMax);

                if (BoardSize < boardSideLengthMin || BoardSize > boardSideLengthMax)
                {
                    Errors.Add($"Board side length must be between {boardSideLengthMin}-{boardSideLengthMax}.");
                }
                
                if (GridSize < boardSideLengthMin || GridSize > BoardSize)
                {
                    Errors.Add($"Grid size must be between {boardSideLengthMin}-{BoardSize}. " +
                               $"(Based on your board size)");
                }
                
                Settings.NewConfigRules.TryGetValue("winConditionLengthMin", out var winConditionLengthMin);
                
                if (WinCondition < winConditionLengthMin || WinCondition > GridSize)
                {
                    Errors.Add($"Winning condition must be between {winConditionLengthMin}-{GridSize}. " +
                               $"(Based on your grid size)");
                }
                
                Settings.NewConfigRules.TryGetValue("movePiecesAfterMin", out var movePiecesAfterMin);
                Settings.NewConfigRules.TryGetValue("movePiecesAfterMax", out var movePiecesAfterMax);
                
                if (MovePiecesAfterNMoves < movePiecesAfterMin || MovePiecesAfterNMoves > movePiecesAfterMax)
                {
                    Errors.Add($"Move pieces after N moves must be between {movePiecesAfterMin}-{movePiecesAfterMax}."); 
                }
                
                if (NumberOfPiecesPerPlayer < WinCondition || NumberOfPiecesPerPlayer > BoardSize * BoardSize / 2 + 1)
                {
                    Errors.Add($"Number of pieces per player must be between " +
                               $"{WinCondition}-{BoardSize * BoardSize / 2 + 1}. (Based on your configuration)");
                }
                
                if (Errors.Count > 0)
                {
                    return RedirectToPage("./Options", new { userName = UserName, errors = Errors, 
                        newConfigName = NewConfigName, boardSize = BoardSize, gridSize = GridSize, 
                        winCondition = WinCondition, whoStarts = WhoStarts, movePiecesAfterNMoves = MovePiecesAfterNMoves, 
                        numberOfPiecesPerPlayer = NumberOfPiecesPerPlayer});
                }
                
                _configRepository.AddConfiguration(NewConfigName, BoardSize, GridSize, WinCondition, 
                    WhoStarts == "X" ? EGamePiece.X : EGamePiece.O, MovePiecesAfterNMoves, NumberOfPiecesPerPlayer);
                
                Success = "Configuration created!";
            }
            else if (Request.Form.ContainsKey("deleteGame"))
            {
                if (_gameRepository.GetGameNamesForUser(UserName).Count == 0)
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
