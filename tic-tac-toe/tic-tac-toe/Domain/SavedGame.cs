using System.ComponentModel.DataAnnotations;

namespace Domain;

public class SavedGame
{
    // Primary key
    public int Id { get; set; }

    [MaxLength(128)]
    public string CreatedAtDateTime { get; set; } = default!;
    
    // Arvuta välja max string length mis saab olla gamestate puhul ja pane maxlength peale?
    [MaxLength(10240)]
    public string State { get; set; } = default!;

    // Expose the Foreign Key
    public int ConfigurationId { get; set; }
    public Configuration? Configuration { get; set; }
}