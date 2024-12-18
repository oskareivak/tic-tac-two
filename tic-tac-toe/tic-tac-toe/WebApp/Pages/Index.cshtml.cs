using ConsoleApp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)] 
    public string? Error { get; set; }
    
    [BindProperty] 
    public string? UserName { get; set; }
    
    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        UserName = UserName?.Trim();
        
        if (!string.IsNullOrWhiteSpace(UserName) && !Settings.RestrictedUsernames.Contains(UserName.ToLower()) 
            && UserName.Length <= Settings.MaxUsernameLength)
        {
            return RedirectToPage("./Home", new { userName = UserName });
        }

        Error = "Please enter a valid username.";
        
        return Page();   
    }
}