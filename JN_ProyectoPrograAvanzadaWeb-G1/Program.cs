using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using JN_ProyectoPrograAvanzadaWeb_G1.Data;
using JN_ProyectoPrograAvanzadaWeb_G1.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; 
    });

// consumir el API
builder.Services.AddHttpClient("ApiClient", client =>
{
    var apiUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001";
    client.BaseAddress = new Uri(apiUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor();

// Registrar servicios HTTP client para consumir el API
builder.Services.AddScoped<IApiAuthService, ApiAuthService>();
builder.Services.AddScoped<IApiUsuarioService, ApiUsuarioService>();
builder.Services.AddScoped<IApiProductoService, ApiProductoService>();
builder.Services.AddScoped<IApiBodegaService, ApiBodegaService>();
builder.Services.AddScoped<IApiRolService, ApiRolService>();
builder.Services.AddScoped<IApiInventarioService, ApiInventarioService>();
builder.Services.AddScoped<IApiMovimientoService, ApiMovimientoService>();
builder.Services.AddScoped<IApiSolicitudService, ApiSolicitudService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Autenticacion/Login";
        options.LogoutPath = "/Autenticacion/Logout";
        options.AccessDeniedPath = "/Autenticacion/Login";
        
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        
        options.Cookie.Name = "InvTech.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() 
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        
        options.SessionStore = null;
        options.Events.OnValidatePrincipal = context =>
        {
            return Task.CompletedTask;
        };
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();
