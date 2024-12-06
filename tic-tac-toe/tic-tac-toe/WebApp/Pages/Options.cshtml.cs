using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

// namespace WebApp.Pages;

// public class Options : PageModel
// {
//     private readonly IConfigRepository _configRepository;
//
//     public Options(IConfigRepository configRepository)
//     {
//         _configRepository = configRepository;
//     }
//
//     // Bindproperty voib ara votta, aga sel juhul peab panema OnGet sisse parameetri string userName ja
//     // OnGet meetodis panna UserName = userName;
//     [BindProperty(SupportsGet = true)] public string UserName { get; set; } = default!;
//     
//     [BindProperty(SupportsGet = true)] public string? Error { get; set; }
//     
//     public SelectList ConfigSelectList { get; set; } = default!;
//
//     // [BindProperty] public int ConfigurationId { get; set; }
//     
//     [BindProperty] public string ConfigurationName { get; set; } = default!;
//     
//     // [BindProperty] public bool IsNewGame { get; set; }
//     
//     public IActionResult OnGet()
//     {
//         if (string.IsNullOrEmpty(UserName))
//         {
//             return RedirectToPage("./Index", new { error = "No username provided." });
//         }
//         
//         ViewData["UserName"] = UserName;
//
//         var selectListData = _configRepository.GetConfigurationNames()
//             .Select(name => new {id = name, value = name})
//             .ToList();
//         
//         ConfigSelectList = new SelectList(selectListData, "id", "value");
//         
//         return Page();
//     }
//     
//     public IActionResult OnPost()
//     {
//         UserName = UserName.Trim();
//
//         if (!string.IsNullOrWhiteSpace(UserName))
//         {
//             return RedirectToPage("./Gameplay", new { userName = UserName, configName = ConfigurationName , IsNewGame = true });
//         }
//
//         Error = "Please enter a username.";
//
//         return RedirectToPage("./Home", new { error = Error });
//     }
//     
// }
