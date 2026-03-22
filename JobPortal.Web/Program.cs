using Dapper_ORM.Services;
using JobPortal.Domain.Models;
using JobPortal.Infrastructure.Data;
using JobPortal.Infrastructure.Seeder;
using JobPortal.Infrastructure.Services;
using JobPortal.Models;
using JobPortal.Web.Files;
using JobPortal.Web.Middlewares;
using JobPortal.Web.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;


Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.File("logs/startup-log-.txt", rollingInterval: RollingInterval.Day)
     .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

// Add session services
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".GreenJobs.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(120); // Set session timeout
    options.Cookie.HttpOnly = false; // Make session cookie accessible only via HTTP
    options.Cookie.IsEssential = true; // Ensure the cookie is available even without consent

    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Use HTTPS
});

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options => { options.SignIn.RequireConfirmedAccount = false; })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    // Redirect when user is not authenticated (login)
    options.LoginPath = "/Account/Login";            // default: /Account/Login
    options.ReturnUrlParameter = "returnUrl";

    // Redirect when user is authenticated but not allowed (403)
     options.AccessDeniedPath = "/Account/Login";
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 20 * 1024 * 1024;
});

// services
builder.Services.AddScoped<IDapper, Dapperr>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<DbSeeder>();
builder.Services.AddScoped<IFileStorage, LocalFileStorage>();
builder.Services.AddScoped<ICompareServices, CompareServices>();
builder.Services.AddScoped<IJLocationServices, JLocationServices>();
builder.Services.AddScoped<IPollModel,PollModel>();
builder.Services.AddScoped<IServey, ServeyServices>();
builder.Services.AddScoped<ILibrary, LibraryServices>();
builder.Services.AddScoped<ILibraryCategory, LibraryCategoryServices>();
builder.Services.AddScoped<IDForms, DFormServices>();
builder.Services.AddScoped<IOneMinute, OneMinuteServices>();
builder.Services.AddScoped<IDFormCategory, DFormCategoryServices>();
builder.Services.AddScoped<IContactForms, ContactFormsServices>();
builder.Services.AddScoped<IWpPostModel, WpPostModel>();
builder.Services.AddScoped<ILeadershipReflection, LeadershipReflectionServices>();
builder.Services.AddScoped<ITimePassCategory, TimePassCategoryServices>();
builder.Services.AddScoped<ICuratedJobs, CuratedJobServices>();
builder.Services.AddScoped<IMyform, MyformServices>();
builder.Services.AddScoped<IFormFeeRepository, FormFeeRepository>();
builder.Services.AddScoped<IFormPaymentRepository, FormPaymentRepository>();

builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 104857600; // 100 MB max if you need large files
});


// 1. Define the path to your secure folder
var keysFolder = Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys");

// 2. Configure Data Protection
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
    .SetApplicationName("GreenJobsDigitalApp"); // Use a unique name for your app


var app = builder.Build();

//app.Use(async (context, next) =>
//{
//    // Only set for successful responses; you may tune for static files separately
//    context.Response.OnStarting(() =>
//    {
//        context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, proxy-revalidate";
//        context.Response.Headers["Pragma"] = "no-cache";
//        context.Response.Headers["Expires"] = "0";
//        context.Response.Headers["Vary"] = "Accept-Encoding";
//        return Task.CompletedTask;
//    });

//    await next();
//});

//// Seed DB
//using (var scope = app.Services.CreateScope())
//{
//    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
//    await seeder.SeedAsync();
//}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
//app.UseAuthorization();
//app.UseMiddleware<ErrorLoggingMiddleware>();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "surveyareroute",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}/{sheetId?}/{blockId?}/{answerId?}");

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
      name: "surveyroute",
      pattern: "{controller=Home}/{action=Index}/{id?}/{stepId?}/{questionid?}");

app.MapControllerRoute(name: "visitorpoll", pattern: "{controller=Home}/{action=Index}/{id?}/{page?}/{qid?}");

app.UseSession();

app.Run();
