using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using NoteFeature_App.Data;
using NoteFeature_App.Middleware;
using NoteFeature_App.Repositories;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDBContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddHostedService<UpdateNoteStatusService>();

//Set time culture
var cultureInfo = new CultureInfo("en-US");
cultureInfo.DateTimeFormat.Calendar = new GregorianCalendar();
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

builder.Services.AddAuthorization();

builder.Services.AddScoped<INoteRepo, NoteRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
//wwwroot
app.UseStaticFiles();

//mny upload
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Upload")),
    RequestPath = "/Upload"
});
app.UseRouting();

app.UseMiddleware<JwtMiddleware>();

app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsProduction())
{
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Account}/{action=Login}");
} else
{
    app.MapControllerRoute(
       name: "default",
       pattern: "{controller=Account}/{action=Login}");
}

app.Run();
