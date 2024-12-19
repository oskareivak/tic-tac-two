using Common;
using DAL;
using Microsoft.EntityFrameworkCore;

var dbPath = Path.Combine(FileHelper.BasePath, "app.db"); 
if (Settings.Mode == ESavingMode.Database && !File.Exists(dbPath))
{ 
    var contextFactory = new AppDbContextFactory();
    var context = contextFactory.CreateDbContext([]);
    context.Database.Migrate();
}

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

connectionString = connectionString.Replace("PLACEHOLDER", $"{FileHelper.BasePath}app.db");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

if (Settings.Mode == ESavingMode.Json)
{
    builder.Services.AddScoped<IConfigRepository, ConfigRepositoryJson>();
    builder.Services.AddScoped<IGameRepository, GameRepositoryJson>();
}
else if (Settings.Mode == ESavingMode.Database) {
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
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages().WithStaticAssets();

app.Run();