using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineMovieTicket.DAL.Data;
using OnlineMovieTicket.DAL.Models;
using OnlineMovieTicket.DAL.SeedData;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.BL.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;

    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;           
    options.Password.RequireLowercase = true;      
    options.Password.RequireUppercase = true;      
    options.Password.RequireNonAlphanumeric = true;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;

    options.User.RequireUniqueEmail = true;

})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var googleAuth = builder.Configuration.GetSection("Authentication:Google");
builder.Services.AddAuthentication()
    .AddCookie(option =>{
        option.LoginPath = "/Account/Login";
        option.LogoutPath = "/Account/Logout";
        option.AccessDeniedPath = "/Account/AccessDenied";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        option.SlidingExpiration = true;
    })
    .AddGoogle(option => {
        option.ClientId = googleAuth["ClientId"] ?? throw new InvalidOperationException("Google ClientId is missing!");
        option.ClientSecret = googleAuth["ClientSecret"] ?? throw new InvalidOperationException("Google ClientSecret is missing!");
        option.CallbackPath = "/signin-google";
        option.SignInScheme = IdentityConstants.ExternalScheme;
        option.Events.OnRemoteFailure = context =>
        {
            context.Response.Redirect("/Identity/Account/Login");
            context.HandleResponse();
            return Task.CompletedTask;
        };
    });

builder.Services.AddScoped<RoleSeeder>();
builder.Services.AddScoped<UserSeeder>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

using (var scope = app.Services.CreateScope()) 
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var roleSeeder = services.GetRequiredService<RoleSeeder>();
        var userSeeder = services.GetRequiredService<UserSeeder>();

        await roleSeeder.SeedRoleAsync();
        await userSeeder.SeedAdminUser();

        logger.LogInformation("Seeding completed successfully!");
    }
    catch (Exception ex)
    {
        logger.LogError($"Error during seeding: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
