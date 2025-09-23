using Microsoft.EntityFrameworkCore;
using NoteFeature_App.Repositories;
using NoteFeature_App.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDBContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<INoteRepo, NoteRepo>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

if (app.Environment.IsProduction())
{
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Login}/{action=Index}");
} else
{
    app.MapControllerRoute(
       name: "default",
       pattern: "{controller=Login}/{action=Index}");
}

app.Run();
