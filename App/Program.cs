using Microsoft.EntityFrameworkCore;
using App.Data;
using App.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Items}/{action=Index}/{id?}");

// Ensure DB and seed sample data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Items.Any())
    {
        db.Items.AddRange(
            new Item { Name = "Sample item 1", Description = "This is a demo item." },
            new Item { Name = "Sample item 2", Description = "Another demo item." }
        );
        db.SaveChanges();
    }
}

app.Run();
