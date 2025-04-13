using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineMovieTicket.DAL.Data;
using OnlineMovieTicket.DAL.Models;
using OnlineMovieTicket.DAL.SeedData;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.BL.Services;
using Microsoft.AspNetCore.Mvc;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Repositories;
using OnlineMovieTicket.BL.Mapping;
using OnlineMovieTicket.DAL.Configurations;


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
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>{
    options.Conventions.ConfigureFilter(new AutoValidateAntiforgeryTokenAttribute());
});

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

builder.Services.AddCors(options => {
    options.AddPolicy("AllowSpecific", policy =>{
        policy.WithOrigins("https://localhost:7019")
                .AllowAnyMethod()
                .AllowAnyHeader();
    });
});

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddScoped<RoleSeeder>();
builder.Services.AddScoped<UserSeeder>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICinemaRepository, CinemaRepository>();
builder.Services.AddScoped<ICinemaService, CinemaService>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<ICloudinaryService, CloudinaryService>();


builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
});

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

app.UseCors("AllowSpecific");
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
