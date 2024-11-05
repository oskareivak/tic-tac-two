using System.Text.Json;
using GameBrain;

namespace DAL;

public class ConfigRepositoryJson : IConfigRepository
{
    public List<string> GetConfigurationNames()
    {
        CheckAndCreateInitialConfigs();

        var result = new List<string>();
        foreach (var fullFileName in Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.ConfigExtension))
        {
            var fileNameParts = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fullFileName));
            result.Add(fileNameParts);
        }

        return result;
    }

    public GameConfiguration GetConfigurationByName(string name)
    {
        var configJsonStr = File.ReadAllText(FileHelper.BasePath + name + FileHelper.ConfigExtension);
        var config = System.Text.Json.JsonSerializer.Deserialize<GameConfiguration>(configJsonStr);
        return config;
    }

    public void AddConfiguration(string name, int boardSize, int gridSize, int winCondition, EGamePiece whoStarts,
        int movePieceAfterNMoves, int numberOfPiecesPerPlayer)
    {
        var newConfig = new GameConfiguration
        {
            Name = name,
            BoardSize = boardSize,
            GridSize = gridSize,
            WinCondition = winCondition,
            WhoStarts = whoStarts,
            MovePieceAfterNMoves = movePieceAfterNMoves,
            NumberOfPiecesPerPlayer = numberOfPiecesPerPlayer
        };

        var configJsonStr = JsonSerializer.Serialize(newConfig);
        File.WriteAllText(FileHelper.BasePath + name + FileHelper.ConfigExtension, configJsonStr);
    }
    
    public void DeleteConfiguration(string name)
    {
        var fileToDelete = FileHelper.BasePath + name + FileHelper.ConfigExtension;
        if (File.Exists(fileToDelete))
        {
            File.Delete(fileToDelete);
        }
        else
        {
            Console.WriteLine("Cannot find the file to delete.");
        }
    }
    
    private void CheckAndCreateInitialConfigs()
    {
        if (!Directory.Exists(FileHelper.BasePath))
        {
            Directory.CreateDirectory(FileHelper.BasePath);
        }
        
        var data = Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.ConfigExtension).ToList();
        
        if (data.Count == 0)
        {
            var hardCodedRepo = new ConfigRepositoryInMemory();
            var optionNames = hardCodedRepo.GetConfigurationNames();
            foreach (var optionName in optionNames)
            {
                var gameOption = hardCodedRepo.GetConfigurationByName(optionName);
                var optionJsonStr = System.Text.Json.JsonSerializer.Serialize(gameOption);
                File.WriteAllText(FileHelper.BasePath + gameOption.Name + FileHelper.ConfigExtension, optionJsonStr);
            }
        }
        
    }
}

// getconfigurationbyname tee nullable siin ehk kui !file.exists siis tuleb null ja gameloopis ytleb, et sorri ei saa
// seda mangu alustada ja vali midagi muud      ??
// ehk seal palju validationit et keegi ei save mangu ja siis muuda asju ja reload vms