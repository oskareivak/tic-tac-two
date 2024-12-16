using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Configuration
{
    // Primary key
    public int Id { get; set; }
    
    [MaxLength(128)]
    public string Name { get; set; } = default!;

    public int BoardSize { get; set; }
    
    public int GridSize { get; set; }
    
    public int WinCondition { get; set; }
    
    public int WhoStarts { get; set; }
    
    public int MovePieceAfterNMoves { get; set; }
    
    public int NumberOfPiecesPerPlayer { get; set; }
    
    public string ConfigOwner { get; set; } = default!;
    
    public ICollection<SavedGame>? SavedGames { get; set; }

    public override string ToString()
    {
        return Id + " " + Name + "(" + BoardSize + "x" + BoardSize + ") Games: " + (SavedGames?.Count.ToString() ?? "not joined");
    }
}