using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proyecto_POO_IIPAC_2026.Data;

var builder = WebApplication.CreateBuilder(args);


// ==========================================
// BASE DE DATOS
// ==========================================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=medicamentos.db"));


// ==========================================
// CONTROLADORES
// ==========================================

builder.Services.AddControllers();


// ==========================================
// ASP.NET IDENTITY
// ==========================================

builder.Services
    .AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


// ==========================================
// CONFIGURACIÓN DE COOKIE
// ==========================================

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/api/account/login";
    options.AccessDeniedPath = "/api/account/access-denied";

    // Evita que una petición de API sea redirigida
    // a una página HTML de login.
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});


var app = builder.Build();


// ==========================================
// CREAR ROLES Y USUARIOS
// ==========================================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager =
        services.GetRequiredService<RoleManager<IdentityRole>>();

    var userManager =
        services.GetRequiredService<UserManager<IdentityUser>>();


    // ==========================================
    // CREAR ROLES
    // ==========================================

    string[] roles =
    {
        "Doctor",
        "Enfermero"
    };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(
                new IdentityRole(role));
        }
    }


    // ==========================================
    // CREAR DOCTOR
    // ==========================================

    string doctorEmail = "doctor@gmail.com";
    string doctorPassword = "Doctor123!";

    var doctor =
        await userManager.FindByEmailAsync(doctorEmail);

    if (doctor == null)
    {
        doctor = new IdentityUser
        {
            UserName = doctorEmail,
            Email = doctorEmail,
            EmailConfirmed = true
        };

        var resultado =
            await userManager.CreateAsync(
                doctor,
                doctorPassword);

        if (resultado.Succeeded)
        {
            await userManager.AddToRoleAsync(
                doctor,
                "Doctor");
        }
    }
    else
    {
        if (!await userManager.IsInRoleAsync(
                doctor,
                "Doctor"))
        {
            await userManager.AddToRoleAsync(
                doctor,
                "Doctor");
        }
    }


    // ==========================================
    // CREAR ENFERMERO
    // ==========================================

    string enfermeroEmail = "enfermero@gmail.com";
    string enfermeroPassword = "Enfermero123!";

    var enfermero =
        await userManager.FindByEmailAsync(enfermeroEmail);

    if (enfermero == null)
    {
        enfermero = new IdentityUser
        {
            UserName = enfermeroEmail,
            Email = enfermeroEmail,
            EmailConfirmed = true
        };

        var resultado =
            await userManager.CreateAsync(
                enfermero,
                enfermeroPassword);

        if (resultado.Succeeded)
        {
            await userManager.AddToRoleAsync(
                enfermero,
                "Enfermero");
        }
    }
    else
    {
        if (!await userManager.IsInRoleAsync(
                enfermero,
                "Enfermero"))
        {
            await userManager.AddToRoleAsync(
                enfermero,
                "Enfermero");
        }
    }
}


// ==========================================
// MIDDLEWARE
// ==========================================

app.UseAuthentication();

app.UseAuthorization();


// ==========================================
// CONTROLADORES
// ==========================================

app.MapControllers();

app.Run();