using Microsoft.EntityFrameworkCore;
using Pustok_BackProject.DataAccessLayer;
using Pustok_BackProject.Interfaces;
using Pustok_BackProject.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews().AddNewtonsoftJson()
    .AddNewtonsoftJson(options=>options.SerializerSettings.ReferenceLoopHandling =Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddScoped<ILayoutService, LayoutService>();

var app = builder.Build();


app.UseStaticFiles();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");


app.Run();
