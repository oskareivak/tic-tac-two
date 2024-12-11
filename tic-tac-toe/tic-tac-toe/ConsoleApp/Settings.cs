namespace ConsoleApp;

public static class Settings
{   
    // Choose the saving mode for game. Options: Memory, Json, Database
    public const ESavingMode Mode = ESavingMode.Json;

    public const int MaxSavedConfigs = 103;
    
    public static readonly Dictionary<string, int> NewConfigRules = new()
    {
        { "gameNameLengthMin" , 1 },
        { "gameNameLengthMax" , 30 },
        { "boardSideLengthMin" , 3 },
        { "boardSideLengthMax" , 20 },
        { "winConditionLengthMin" , 3 },
        { "movePiecesAfterMin" , 0 },
        { "movePiecesAfterMax" , 170 }
    };
}