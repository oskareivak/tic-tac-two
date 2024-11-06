using System;

namespace MenuSystem;

public class MenuItem
{
    private string _title = default!;
    
    private string _shortcut = default!;

    public override string ToString()
    {
        return $"{_shortcut}) {_title}";
    }

    public Func<string>? MenuItemAction { get; set; }

    public string Title
    {
        get => _title;
        set 
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Title cannot be empty");
            }
            _title = value;
        }
    }

    public string Shortcut
    {
        get => _shortcut;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Shortcut cannot be empty.");
            }
            _shortcut = value;
        }
    }
    
}



