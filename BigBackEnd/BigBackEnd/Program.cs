using BigBackEnd.DataAccessLayer;
using BigBackEnd.Extensions;
using BigBackEnd.Interfaces;
using BigBackEnd.Models;
using BigBackEnd.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(option=>option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase= true;
    options.Password.RequireDigit= true;
    options.Password.RequireNonAlphanumeric= false;
    options.Password.RequiredLength= 8;
    options.User.RequireUniqueEmail = true;
    
    options.Lockout.AllowedForNewUsers = false;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.MaxFailedAccessAttempts = 3;


}).AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders().AddErrorDescriber<IdentityErrorDescriberAZ>();


builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ILayoutService, LayoutService>();

var app = builder.Build();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();


app.UseStaticFiles();

app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
          );
app.MapControllerRoute("default", "{controller=home}/{action=index}/{id?}");


app.Run();
