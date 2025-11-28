using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Singula.Core.Infrastructure.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===============================================
// CONTROLLERS
// ===============================================
builder.Services.AddControllers();

// ===============================================
// SWAGGER + Soporte para IFormFile + CORS
// ===============================================
builder.Services.AddEndpointsApiExplorer(); // De Master

// 1. CONFIGURACIÓN DE CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp",
        policy => policy
            .AllowAnyOrigin()   // Permite conexiones desde cualquier puerto
            .AllowAnyMethod()   // Permite GET, POST, PUT, DELETE
            .AllowAnyHeader()); // Permite enviar Tokens en la cabecera
});

builder.Services.AddOptions(); // De Feature

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Singula API",
        Version = "v1"
    });

    // Soporte para archivos en Swagger
    options.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });

    options.SupportNonNullableReferenceTypes();
});

// ===============================================
// LIMITE DE SUBIDA (50 MB)
// ===============================================
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50_000_000; // 50MB
});

// ===============================================
// DATABASE
// ===============================================
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(conn)
);

// ===============================================
// REPOS + SERVICES
// ===============================================
builder.Services.AddScoped(typeof(Singula.Core.Repositories.IRepository<>), typeof(Singula.Core.Repositories.EfRepository<>));
builder.Services.AddScoped<Singula.Core.Repositories.IUsuarioRepository, Singula.Core.Repositories.UsuarioRepository>();
builder.Services.AddScoped<Singula.Core.Repositories.IAlertumRepository, Singula.Core.Repositories.AlertumRepository>();

builder.Services.AddScoped<Singula.Core.Services.IUsuarioService, Singula.Core.Services.UsuarioService>();
builder.Services.AddScoped<Singula.Core.Services.IAreaService, Singula.Core.Services.AreaService>();
builder.Services.AddScoped<Singula.Core.Services.IAlertumService, Singula.Core.Services.AlertumService>();
builder.Services.AddScoped<Singula.Core.Services.IRolRegistroService, Singula.Core.Services.RolRegistroService>();
builder.Services.AddScoped<Singula.Core.Services.IRolesSistemaService, Singula.Core.Services.RolesSistemaService>();
builder.Services.AddScoped<Singula.Core.Services.ITipoSolicitudCatalogoService, Singula.Core.Services.TipoSolicitudCatalogoService>();
builder.Services.AddScoped<Singula.Core.Services.IPrioridadCatalogoService, Singula.Core.Services.PrioridadCatalogoService>();
builder.Services.AddScoped<Singula.Core.Services.IReporteService, Singula.Core.Services.ReporteService>();
builder.Services.AddScoped<Singula.Core.Services.IConfigSlaService, Singula.Core.Services.ConfigSlaService>();
builder.Services.AddScoped<Singula.Core.Services.ITipoAlertaCatalogoService, Singula.Core.Services.TipoAlertaCatalogoService>();
builder.Services.AddScoped<Singula.Core.Services.IEstadoSolicitudCatalogoService, Singula.Core.Services.EstadoSolicitudCatalogoService>();
builder.Services.AddScoped<Singula.Core.Services.IEstadoAlertaCatalogoService, Singula.Core.Services.EstadoAlertaCatalogoService>();
builder.Services.AddScoped<Singula.Core.Services.IEstadoUsuarioCatalogoService, Singula.Core.Services.EstadoUsuarioCatalogoService>();
builder.Services.AddScoped<Singula.Core.Services.IPermisoService, Singula.Core.Services.PermisoService>();
builder.Services.AddScoped<Singula.Core.Services.ISolicitudService, Singula.Core.Services.SolicitudService>();
builder.Services.AddScoped<Singula.Core.Services.IPersonalService, Singula.Core.Services.PersonalService>();
builder.Services.AddScoped<Singula.Core.Services.IDashboardService, Singula.Core.Services.DashboardService>();
builder.Services.AddScoped<Singula.Core.Services.IExcelImportService, Singula.Core.Services.ExcelImportService>();

// JWT AUTHENTICATION
// ===============================================
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = jwtSection["Key"];
var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

// ===============================================
// Kestrel: PUERTOS FIJOS
// ===============================================
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:7002");

var app = builder.Build();

// Seed DB with retry logic
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    int retries = 0;
    int maxRetries = 10;
    
    while (retries < maxRetries)
    {
        try
        {
            SeedData.EnsureSeedData(db);
            break;
        }
        catch (Exception ex)
        {
            retries++;
            if (retries >= maxRetries)
            {
                Console.WriteLine($"Failed to seed database after {maxRetries} attempts: {ex.Message}");
                throw;
            }
            System.Threading.Thread.Sleep(2000); 
            Console.WriteLine($"Database not ready, retrying... ({retries}/{maxRetries})");
        }
    }
}

// ===============================================
// HTTP REQUEST PIPELINE
// ===============================================

app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection(); // Opcional, según tu entorno

app.UseStaticFiles();

// 2. ACTIVACIÓN DE CORS
// IMPORTANTE: Debe ir ANTES de UseAuthentication y UseAuthorization
// Usamos la política "AllowWebApp" definida arriba que permite todo
app.UseCors("AllowWebApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();