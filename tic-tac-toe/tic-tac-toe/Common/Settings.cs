using GameBrain;
namespace Common;

public static class Settings
{   
    // Choose the saving mode for app. Options: Json, Database
    public const ESavingMode Mode = ESavingMode.Json;
    
    
    public const int MaxSavedConfigsPerUser = 10;
    public const int MaxSavedGamesPerUser = 30;
    
    public const int MaxUsernameLength = 10;
    
    public const int AiDelayMin = 600;
    public const int AiDelayMax = 1000;
    
    public static readonly IReadOnlyDictionary<string, int> NewConfigRules = new Dictionary<string, int>
    {
        { "gameNameLengthMin" , 1 },
        { "gameNameLengthMax" , 30 },
        { "boardSideLengthMin" , 3 },
        { "boardSideLengthMax" , 12 },
        { "winConditionLengthMin" , 3 },
        { "movePiecesAfterMin" , 0 },
        { "movePiecesAfterMax" , 170 }
    };
    
    public static readonly IReadOnlySet<string> RestrictedUsernames = new HashSet<string>
    {
        "ai", 
        "ai1", 
        "ai2", 
        "....",
        "game"
    };

    public static readonly IReadOnlyDictionary<EGameMode, string> GameModeStrings = new Dictionary<EGameMode, string>
    {
        { EGameMode.PvP, "Player vs Player" },
        { EGameMode.PvAi, "Player vs AI" },
        { EGameMode.AivAi, "AI vs AI" }
    };
    
}