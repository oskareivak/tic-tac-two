using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages;

public class Home : PageModel
{
    private readonly IConfigRepository _configRepository;

    public Home(IConfigRepository configRepository)
    {
        _configRepository = configRepository;
    }

    // Bindproperty voib ara votta, aga sel juhul peab panema OnGet sisse parameetri string userName ja
    // OnGet meetodis panna UserName = userName;
    [BindProperty(SupportsGet = true)]
    public string UserName { get; set; } = default!;

    public SelectList ConfigSelectList { get; set; } = default!;

    [BindProperty]
    public int ConfigurationId { get; set; }
    
    public IActionResult OnGet()
    {
        if (string.IsNullOrEmpty(UserName))
        {
            return RedirectToPage("./Index", new { error = "No username provided." });
        }
        
        ViewData["UserName"] = UserName;

        var selectListData = _configRepository.GetConfigurationNames()
            .Select(name => new {id = name, value = name})
            .ToList();
        
        ConfigSelectList = new SelectList(selectListData, "id", "value");
        
        return Page();
    }
    
}