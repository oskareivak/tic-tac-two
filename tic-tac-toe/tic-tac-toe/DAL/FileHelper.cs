namespace DAL;

public static class FileHelper
{
    public static string BasePath { get; } = ResolveBasePath();

    public static string ConfigExtension = ".config.json";
    public static string GameExtension = ".game.json";

    private static string ResolveBasePath()
    {
        var envPath = Environment.GetEnvironmentVariable("TICTACTWO_DATA_DIR");
        var basePath = string.IsNullOrWhiteSpace(envPath)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "tic-tac-two")
            : envPath.Trim();

        if (!basePath.EndsWith(Path.DirectorySeparatorChar))
        {
            basePath += Path.DirectorySeparatorChar;
        }

        Directory.CreateDirectory(basePath);
        return basePath;
    }
}