using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Singula.Core.Infrastructure.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configuration
builder.Services.AddOptions();

// DbContext
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(conn)
);

// Generic repository registration
builder.Services.AddScoped(typeof(Singula.Core.Repositories.IRepository<>), typeof(Singula.Core.Repositories.EfRepository<>));

// Repositories / Services DI (specific)
builder.Services.AddScoped<Singula.Core.Repositories.IUsuarioRepository, Singula.Core.Repositories.UsuarioRepository>();

// Domain services registration
builder.Services.AddScoped<Singula.Core.Services.IUsuarioService, Singula.Core.Services.UsuarioService>();
builder.Services.AddScoped<Singula.Core.Services.IAreaService, Singula.Core.Services.AreaService>();
builder.Services.AddScoped<Singula.Core.Services.IAlertumService, Singula.Core.Services.AlertumService>();
builder.Services.AddScoped<Singula.Core.Services.IRolRegistroService, Singula.Core.Services.RolRegistroService>();
builder.Services.AddScoped<Singula.Core.Services.IRolesSistemaService, Singula.Core.Services.RolesSistemaService>();
builder.Services.AddScoped<Singula.Core.Services.ITipoSolicitudCatalogoService, Singula.Core.Services.TipoSolicitudCatalogoService>();
builder.Services.AddScoped<Singula.Core.Services.IReporteService, Singula.Core.Services.ReporteService>();
builder.Services.AddScoped<Singula.Core.Services.IConfigSlaService, Singula.Core.Services.ConfigSlaService>();
builder.Services.AddScoped<Singula.Core.Services.ITipoAlertaCatalogoService, Singula.Core.Services.TipoAlertaCatalogoService>();
builder.Services.AddScoped<Singula.Core.Services.IEstadoSolicitudCatalogoService, Singula.Core.Services.EstadoSolicitudCatalogoService>();
builder.Services.AddScoped<Singula.Core.Services.IEstadoAlertaCatalogoService, Singula.Core.Services.EstadoAlertaCatalogoService>();
builder.Services.AddScoped<Singula.Core.Services.IEstadoUsuarioCatalogoService, Singula.Core.Services.EstadoUsuarioCatalogoService>();
builder.Services.AddScoped<Singula.Core.Services.IPermisoService, Singula.Core.Services.PermisoService>();
builder.Services.AddScoped<Singula.Core.Services.ISolicitudService, Singula.Core.Services.SolicitudService>();
builder.Services.AddScoped<Singula.Core.Services.IPersonalService, Singula.Core.Services.PersonalService>();

// JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = jwtSection["Key"];
var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"];

if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
    throw new Exception("JWT not configured in appsettings.json");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

var app = builder.Build();

// Seed DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    SeedData.EnsureSeedData(db);
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
