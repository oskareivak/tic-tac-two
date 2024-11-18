using Microsoft.EntityFrameworkCore;
using DAL;
using ConsoleApp;

// Initialize and migrate the database if the settings mode is database and the database doesn't exist.
var dbPath = Path.Combine(FileHelper.BasePath, "app.db"); 
if (Settings.Mode == ESavingMode.Database && !File.Exists(dbPath))
{ 
    var contextFactory = new AppDbContextFactory();
    var context = contextFactory.CreateDbContext([]);
    // Use the correct path for the database
    context.Database.Migrate();
}

// Run the main menu
Menus.MainMenu.Run();