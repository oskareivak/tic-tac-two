using Common;
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
        if (string.IsNullOrWhiteSpace(UserName) || Settings.RestrictedUsernames.Contains(UserName.ToLower()) 
                                                || UserName.Length > Settings.MaxUsernameLength)
        {
            return RedirectToPage("./Index", new { error = "Invalid username provided." });
        }
        
        ViewData["UserName"] = UserName;

        
        var gameSelectListData = _gameRepository.GetGameIdNamePairsForUser(UserName)
            .Select(pair => new { id = pair.Key, value = pair.Value })
            .ToList();
        
        GameSelectList = new SelectList(gameSelectListData, "id", "value");
        
        
        var configRepositoryInMemory = new ConfigRepositoryInMemory();
        var defaultConfigurations = configRepositoryInMemory.GetConfigurationNames();
        
        var configSelectListData = _configRepository.GetOnlyUserConfigIdNamePairsForUser(UserName)
            .Select(pair => new { id = pair.Key, value = pair.Value })
            .ToList();
        
        ConfigSelectList = new SelectList(configSelectListData, "id", "value");
        
        return Page();
    }
    
    public IActionResult OnPost()
    {
        Errors.Clear();
        
        UserName = UserName.Trim();
    
        if (!string.IsNullOrWhiteSpace(UserName) && !Settings.RestrictedUsernames.Contains(UserName.ToLower()) 
                                                 && UserName.Length <= Settings.MaxUsernameLength)
        {
            var savedConfigsCount = _configRepository.GetOnlyUserConfigIdNamePairsForUser(UserName).Count;
            Console.WriteLine("savedconfigscount" + savedConfigsCount); // TODO: delete
            
            if (Request.Form.ContainsKey("deleteConfiguration"))
            {
                
                if (!(savedConfigsCount > 0))
                {
                    Error = "You don't have any configurations to delete yet!";
                    return RedirectToPage("./Options", new { userName = UserName, error = Error});
                }
                
                _configRepository.DeleteConfigurationById(ConfigurationId); 
                Success = "Configuration deleted!";
                
            }
            else if (Request.Form.ContainsKey("createConfiguration"))
            {
                var maxSavedConfigs = Settings.MaxSavedConfigsPerUser;
                
                if (savedConfigsCount >= maxSavedConfigs)
                {
                    Error = ($"You have reached the maximum number of saved configurations " +
                                      $"({maxSavedConfigs}). Please delete " +
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

                var existingConfigNames = new List<string>();

                if (Settings.Mode == ESavingMode.Json)
                {
                    existingConfigNames = _configRepository.GetConfigurationNames()
                        .Select(name => name.Split('|').First())   // Split each string and take the first part
                        .Select(part => part.ToLower())            // Convert the first part to lowercase
                        .ToList(); 
                }
                else
                {
                    existingConfigNames = _configRepository.GetConfigurationNames()
                        .Select(name => name.ToLower())
                        .ToList();
                }
                
                if (existingConfigNames.Contains(NewConfigName))
                {
                    Errors.Add($"Configuration name '{NewConfigName}' is already taken.");
                }
                
                Settings.NewConfigRules.TryGetValue("boardSideLengthMin", out var boardSideLengthMin);
                Settings.NewConfigRules.TryGetValue("boardSideLengthMax", out var boardSideLengthMax);

                if (BoardSize < boardSideLengthMin || BoardSize > boardSideLengthMax)
                {
                    Errors.Add($"Board size must be between {boardSideLengthMin}-{boardSideLengthMax}.");
                }
                
                if (GridSize < boardSideLengthMin || GridSize > BoardSize)
                {
                    if (BoardSize == 0)
                    {
                        Errors.Add($"Grid size must be between {boardSideLengthMin}-.... " +
                                   $"(Based on your board size)");
                    }
                    else
                    {
                        Errors.Add($"Grid size must be between {boardSideLengthMin}-{BoardSize}. " +
                                   $"(Based on your board size)");
                    }
                    
                }
                
                Settings.NewConfigRules.TryGetValue("winConditionLengthMin", out var winConditionLengthMin);
                
                if (WinCondition < winConditionLengthMin || WinCondition > GridSize)
                {
                    if (GridSize == 0)
                    {
                        Errors.Add($"Winning condition must be between {winConditionLengthMin}-.... " +
                                   $"(Based on your grid size)");
                    }
                    else
                    {
                        Errors.Add($"Winning condition must be between {winConditionLengthMin}-{GridSize}. " +
                                   $"(Based on your grid size)");
                    }
                    
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
                                                   WhoStarts == "X" ? EGamePiece.X : EGamePiece.O, 
                                                   MovePiecesAfterNMoves, NumberOfPiecesPerPlayer, UserName);
                
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
    
        Error = "Please enter a valid username.";
        
        return RedirectToPage("./Home", new { error = Error });
    }
}
