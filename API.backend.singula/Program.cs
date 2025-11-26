using Microsoft.EntityFrameworkCore;
using Singula.Core.Infrastructure.Data;
using Singula.Core.Infrastructure.Repositories;
using Singula.Core.Infrastructure.Services;
using Singula.Core.Core.Interfaces;
using Singula.Core.Core.Interfaces.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuration: allow a DefaultConnection in appsettings or environment
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();

// Add DbContext (Postgres)
var connectionString = configuration.GetConnectionString("DefaultConnection") ?? configuration["ConnectionStrings:DefaultConnection"] ?? "Host=localhost;Database=TTCBD;Username=postgres;Password=postgres";
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

<<<<<<< Updated upstream
// Repositories / UnitOfWork
builder.Services.AddScoped<IArchivoExcelRepository, ArchivoExcelRepository>();
builder.Services.AddScoped<IRegistroKPIRepository, RegistroKPIRepository>();
builder.Services.AddScoped<IAlertaRepository, AlertaRepository>();
builder.Services.AddScoped<IReporteRepository, ReporteRepository>();
builder.Services.AddScoped<IPrediccionRepository, PrediccionRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
=======
// CORS - allow any origin/header/method
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAny", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// DbContext
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(conn)
);
>>>>>>> Stashed changes

// Infrastructure services
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();
builder.Services.AddScoped<IExcelProcessorService, ExcelProcessorService>();

// Application services
builder.Services.AddScoped<IArchivoExcelService, ArchivoExcelService>();
builder.Services.AddScoped<IRegistroKPIService, RegistroKPIService>();
builder.Services.AddScoped<IPrediccionService, PrediccionService>();

<<<<<<< Updated upstream
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
=======
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
builder.Services.AddScoped<Singula.Core.Services.IDashboardService, Singula.Core.Services.DashboardService>();
builder.Services.AddScoped<Singula.Core.Services.ISlaService, Singula.Core.Services.SlaService>();

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
>>>>>>> Stashed changes

var app = builder.Build();

// Configure the HTTP request pipeline.
<<<<<<< Updated upstream
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
=======
app.UseCors("AllowAny");
>>>>>>> Stashed changes

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
