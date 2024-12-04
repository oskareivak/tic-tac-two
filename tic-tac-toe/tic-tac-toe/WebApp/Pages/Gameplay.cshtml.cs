using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class Gameplay : PageModel
{
    private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;

    public Gameplay(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }
    
    [BindProperty(SupportsGet = true)] public int GameId { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] public EGamePiece NextMoveBy { get; set; } = default!;

    public TicTacTwoBrain TicTacTwoBrain { get; set; } = default!;
    
    public void OnGet()
    {
        var dbGame = _gameRepository.GetSavedGame(GameId);
        
        var gameStateJson = System.Text.Json.JsonSerializer.Deserialize<GameState>(dbGame.State) 
                            ?? throw new Exception("Deserialization failed");

        TicTacTwoBrain = new TicTacTwoBrain(gameStateJson);
        
    }
    
    
}