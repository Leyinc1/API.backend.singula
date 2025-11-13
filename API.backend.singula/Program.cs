using Microsoft.EntityFrameworkCore;
using Singula.Core.Infrastructure.Data;
using Singula.Core.Infrastructure.Repositories;
using Singula.Core.Infrastructure.Services;
using Singula.Core.Core.Interfaces;
using Singula.Core.Core.Interfaces.Services;
using Singula.Core.Core.Settings;

var builder = WebApplication.CreateBuilder(args);

// Configuration: allow a DefaultConnection in appsettings or environment
var configuration = builder.Configuration;

// Bind settings
var emailSettings = new EmailSettings();
configuration.GetSection("EmailSettings").Bind(emailSettings);
builder.Services.AddSingleton(emailSettings);

// Add services to the container.
builder.Services.AddControllers();

// Add DbContext (Postgres)
var connectionString = configuration.GetConnectionString("DefaultConnection") ?? configuration["ConnectionStrings:DefaultConnection"] ?? "Host=localhost;Database=TTCBD;Username=postgres;Password=postgres";
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

// Repositories / UnitOfWork
builder.Services.AddScoped<IArchivoExcelRepository, ArchivoExcelRepository>();
builder.Services.AddScoped<IRegistroKPIRepository, RegistroKPIRepository>();
builder.Services.AddScoped<IAlertaRepository, AlertaRepository>();
builder.Services.AddScoped<IReporteRepository, ReporteRepository>();
builder.Services.AddScoped<IPrediccionRepository, PrediccionRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Infrastructure services
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();
builder.Services.AddScoped<IExcelProcessorService, ExcelProcessorService>();

// Application services
builder.Services.AddScoped<IArchivoExcelService, ArchivoExcelService>();
builder.Services.AddScoped<IRegistroKPIService, RegistroKPIService>();
builder.Services.AddScoped<IPrediccionService, PrediccionService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
