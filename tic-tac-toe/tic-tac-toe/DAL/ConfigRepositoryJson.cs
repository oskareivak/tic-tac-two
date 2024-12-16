using System.Text.Json;
using System.Xml;
using Domain;
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

    public GameConfiguration GetConfigurationByName(string name) // Isn't actually by name. ID is included in the name.
    {
        var configJsonStr = File.ReadAllText(FileHelper.BasePath + name + FileHelper.ConfigExtension);
        var config = System.Text.Json.JsonSerializer.Deserialize<GameConfiguration>(configJsonStr);
        return config;
    }

    public void AddConfiguration(string name, int boardSize, int gridSize, int winCondition, EGamePiece whoStarts,
        int movePieceAfterNMoves, int numberOfPiecesPerPlayer, string configOwner)
    {
        var newConfig = new GameConfiguration
        {
            Name = name,
            BoardSize = boardSize,
            GridSize = gridSize,
            WinCondition = winCondition,
            WhoStarts = whoStarts,
            MovePieceAfterNMoves = movePieceAfterNMoves,
            NumberOfPiecesPerPlayer = numberOfPiecesPerPlayer,
            ConfigOwner = configOwner
        };

        
        var data = Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.ConfigExtension)
            .Select(Path.GetFileNameWithoutExtension)
            .Select(Path.GetFileNameWithoutExtension)
            .ToList();
        
        var existingIds = data
            .Select(config => config!.Split('|').Last())
            .Select(idStr => int.TryParse(idStr, out var id) ? id : (int?)null)
            .Where(id => id.HasValue)
            .Select(id => id.Value)
            .ToList();
        
        var newId = 1;
        while (existingIds.Contains(newId))
        {
            newId++;
        }
        
        var configFileName = $"{name} | {newId}{FileHelper.ConfigExtension}";
        var configJsonStr = JsonSerializer.Serialize(newConfig);
        File.WriteAllText(Path.Combine(FileHelper.BasePath, configFileName), configJsonStr);

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
            throw new Exception($"Cannot find the file to delete with name: {name}");
        }
    }

    public GameConfiguration GetConfigurationById(int id)
    {
        var data = Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.ConfigExtension)
            .Select(Path.GetFileNameWithoutExtension)
            .Select(Path.GetFileNameWithoutExtension)
            .ToList();

        foreach (var configNameWithId in data)
        {
            if (configNameWithId!.Split('|').Last().Trim() == id.ToString())
            {
                var configJsonStr = File.ReadAllText(FileHelper.BasePath + configNameWithId + FileHelper.ConfigExtension);
                var config = System.Text.Json.JsonSerializer.Deserialize<GameConfiguration>(configJsonStr);
                return config;
            }
        }

        throw new Exception($"Configuration not found with id: {id}.");
    }

    public void DeleteConfigurationById(int id)
    {
        var data = Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.ConfigExtension)
            .Select(Path.GetFileNameWithoutExtension)
            .Select(Path.GetFileNameWithoutExtension)
            .ToList();

        foreach (var configNameWithId in data)
        {
            if (configNameWithId!.Split('|').Last().Trim() == id.ToString())
            {
                var fileToDelete = FileHelper.BasePath + configNameWithId + FileHelper.ConfigExtension;
                if (File.Exists(fileToDelete))
                {
                    File.Delete(fileToDelete);
                }
                else
                {
                    throw new Exception($"Cannot find the file to delete with id: {id}.");
                }
            }
        }
    }

    public Dictionary<int, string> GetConfigurationIdNamePairs()
    {
        // CheckAndCreateInitialConfigs(); // TODO: remove
        //
        // var data = Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.ConfigExtension)
        //     .Select(Path.GetFileNameWithoutExtension)
        //     .Select(Path.GetFileNameWithoutExtension)
        //     .ToList();
        //
        // Dictionary<int, string> idNamePairs = new();
        //
        // foreach (var configNameWithId in data)
        // {
        //     var id = int.Parse(configNameWithId!.Split("|").Last().Trim());
        //     var name = configNameWithId.Split("|").First().Trim();
        //     
        //     idNamePairs.Add(id, name);
        // }
        //
        // return idNamePairs;

        return new Dictionary<int, string>();
    }

    public List<string> GetConfigNamesForUser(string username)
    {
        CheckAndCreateInitialConfigs();
        
        var result = new List<string>();

        foreach (var fullFileName in Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.ConfigExtension))
        {
            var configJsonStr = File.ReadAllText(fullFileName);
            var config = System.Text.Json.JsonSerializer.Deserialize<GameConfiguration>(configJsonStr);
            
            if (config.ConfigOwner == username || config.ConfigOwner == "GAME")
            {
                result.Add(Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fullFileName)));
            }
        }

        return result;
    }

    public Dictionary<int, string> GetConfigIdNamePairsForUser(string username)
    {
        CheckAndCreateInitialConfigs();
        
        var data = Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.ConfigExtension)
            .ToList();

        var idNamePairs = new Dictionary<int, string>();
        
        foreach (var fullConfigFileName in data)
        {
            var configJsonStr = File.ReadAllText(fullConfigFileName);
            var config = System.Text.Json.JsonSerializer.Deserialize<GameConfiguration>(configJsonStr);
            
            var fileName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fullConfigFileName));
            
            var id = int.Parse(fileName.Split("|").Last().Trim());
            
            if (config.ConfigOwner == username || config.ConfigOwner == "GAME")
            {
                var name = fileName.Split("|").First().Trim();
                
                idNamePairs.Add(id, name);
            }
        }

        return idNamePairs;
    }

    private void CheckAndCreateInitialConfigs()
    {
        if (!Directory.Exists(FileHelper.BasePath))
        {
            Directory.CreateDirectory(FileHelper.BasePath);
        }

        var data = Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.ConfigExtension)
            .Select(Path.GetFileNameWithoutExtension)
            .ToList();
            
        var existingIds = data
            .Select(config => config!.Split('|').Last())
            .Select(idStr => int.TryParse(idStr, out var id) ? id : (int?)null)
            .Where(id => id.HasValue)
            .Select(id => id.Value)
            .ToList();
        
        var hardCodedRepo = new ConfigRepositoryInMemory();
        var configNames = hardCodedRepo.GetConfigurationNames();

        foreach (var configName in configNames)
        {
            if (!data.Contains(configName))
            {
                var newId = 1;
                while (existingIds.Contains(newId))
                {
                    newId++;
                }

                var gameConfig = hardCodedRepo.GetConfigurationByName(configName);
                var configJsonStr = JsonSerializer.Serialize(gameConfig);
                File.WriteAllText(FileHelper.BasePath + configName + " | " + newId + FileHelper.ConfigExtension, configJsonStr);

                existingIds.Add(newId);
                data.Add(configName + " | " + newId);
                
            }
        }
        
        
        
    }
}

// getconfigurationbyname tee nullable siin ehk kui !file.exists siis tuleb null ja gameloopis ytleb, et sorri ei saa
// seda mangu alustada ja vali midagi muud      ??
// ehk seal palju validationit et keegi ei save mangu ja siis muuda asju ja reload vms