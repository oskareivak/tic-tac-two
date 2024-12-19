using System.ComponentModel.DataAnnotations;

namespace Domain;

public class SavedGame
{
    public int Id { get; set; }

    [MaxLength(128)]
    public string CreatedAtDateTime { get; set; } = default!;
    
    [MaxLength(10240)]
    public string State { get; set; } = default!;
    
    public int ConfigurationId { get; set; }
    
    public string CanDelete1 { get; set; } = string.Empty;
    
    public string CanDelete2 { get; set; } = string.Empty;
    public Configuration Configuration { get; set; } = default!;
}