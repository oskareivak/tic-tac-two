using GameBrain;

namespace ConsoleApp;

public static class Settings
{   
    // Choose the saving mode for game. Options: Memory, Json, Database
    public const ESavingMode Mode = ESavingMode.Database;

    public const int MaxSavedConfigs = 103;
    
    public static readonly IReadOnlyDictionary<string, int> NewConfigRules = new Dictionary<string, int>
    {
        { "gameNameLengthMin" , 1 },
        { "gameNameLengthMax" , 30 },
        { "boardSideLengthMin" , 3 },
        { "boardSideLengthMax" , 20 },
        { "winConditionLengthMin" , 3 },
        { "movePiecesAfterMin" , 0 },
        { "movePiecesAfterMax" , 170 }
    };

    public static readonly IReadOnlyDictionary<EGameMode, string> GameModeStrings = new Dictionary<EGameMode, string>
    {
        { EGameMode.PvP, "Player vs Player" },
        { EGameMode.PvAi, "Player vs AI" },
        { EGameMode.AivAi, "AI vs AI" }
    };

    public const int AiDelayMin = 600;
    public const int AiDelayMax = 1000;
}