using DAL;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using ConsoleApp;
using Settings = ConsoleApp.Settings;

var dbPath = Path.Combine(FileHelper.BasePath, "app.db"); 
if (Settings.Mode == ESavingMode.Database && !File.Exists(dbPath))
{ 
    var contextFactory = new AppDbContextFactory();
    var context = contextFactory.CreateDbContext([]);
    // Use the correct path for the database
    context.Database.Migrate();
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

// Replace the placeholder with the actual path
connectionString = connectionString.Replace("PLACEHOLDER", $"{FileHelper.BasePath}app.db");

// register "how to create a db when somebody asks for it" 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// create new one every time
//builder.Services
//  .addTransient<>(); - create new one every time
//  .addSingleton<>(); - create one and reuse it
//  .addScoped<>(); - create new one per request

if (ConsoleApp.Settings.Mode == ESavingMode.Json)
{
    builder.Services.AddScoped<IConfigRepository, ConfigRepositoryJson>();
    builder.Services.AddScoped<IGameRepository, GameRepositoryJson>();
}
else if (ConsoleApp.Settings.Mode == ESavingMode.Database) {
    builder.Services.AddScoped<IConfigRepository, ConfigRepositoryDb>();
    builder.Services.AddScoped<IGameRepository, GameRepositoryDb>();
}

builder.Services.AddDatabaseDeveloperPageExceptionFilter(); 


builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
} 
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages().WithStaticAssets();

app.Run();