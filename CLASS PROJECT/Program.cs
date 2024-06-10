using DbRepository.Data;
using DbRepository.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.IO; // Ajout de la référence pour utiliser la classe Directory

var builder = WebApplication.CreateBuilder(args);

// Ajoutez les services au conteneur.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

// Créez le dossier wwwroot s'il n'existe pas déjà
var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
if (!Directory.Exists(wwwrootPath))
{
    Directory.CreateDirectory(wwwrootPath);
}

// Configuration pour la connexion à la base de données
var provider = builder.Services.BuildServiceProvider();
var configuration = provider.GetRequiredService<IConfiguration>();
var connectionString = configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Configuration de l'identité
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Ajoutez des politiques d'autorisation
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
});

builder.Services.AddCors(options =>
{
    var frontendUrl = configuration.GetValue<string>("frontendUrl");
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(frontendUrl).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.LoginPath = "/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Configurez le pipeline de requête HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Initialisez les rôles et les utilisateurs
    await InitializeRolesAndUsers(services);
}

app.Run();

async Task InitializeRolesAndUsers(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

    string[] roleNames = { "Admin", "User" };
    IdentityResult roleResult;

    // Créez les rôles s'ils n'existent pas
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Créez un utilisateur Admin par défaut
    var adminUser = new User
    {
        UserName = "admin",
        Email = "admin@example.com",
        EmailConfirmed = true,
    };

    string adminPassword = "Admin@123";

    var user = await userManager.FindByEmailAsync(adminUser.Email);
    if (user == null)
    {
        var createAdminUser = await userManager.CreateAsync(adminUser, adminPassword);
        if (createAdminUser.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    // Créez un utilisateur par défaut
    var defaultUser = new User
    {
        UserName = "user",
        Email = "user@example.com",
        EmailConfirmed = true,
    };

    string userPassword = "User@123";

    var userCheck = await userManager.FindByEmailAsync(defaultUser.Email);
    if (userCheck == null)
    {
        var createDefaultUser = await userManager.CreateAsync(defaultUser, userPassword);
        if (createDefaultUser.Succeeded)
        {
            await userManager.AddToRoleAsync(defaultUser, "User");
        }
    }
}
