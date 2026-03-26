using Microsoft.EntityFrameworkCore;
using App.Data;
using App.Models;
using App.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<QueryExecutorService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<FilePathResolverService>();
builder.Services.AddScoped<FileAccessService>();

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


app.Run();
