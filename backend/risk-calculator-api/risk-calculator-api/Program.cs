using FluentValidation;
using RiskCalculator.API.Services;
using RiskCalculator.API.Validators;
using RiskCalculator.API.Models.Requests;
using RiskCalculator.API.Middleware;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/risk-calculator-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173", 
                "http://localhost:3000", 
                "https://localhost:5173",
                "http://localhost:5174",
                "http://localhost:4173",
                "http://localhost:8080")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Risk Calculator API",
        Version = "v1",
        Description = "Comprehensive cybersecurity risk assessment API for calculating risk using THREAT × VULNERABILITY formula",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Haspod.com",
            Url = new Uri("https://www.haspod.com/")
        }
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CalculateRiskRequestValidator>();

// Add application services
builder.Services.AddScoped<IRiskCalculationService, RiskCalculationService>();

// Add validators
builder.Services.AddScoped<IValidator<CalculateRiskRequest>, CalculateRiskRequestValidator>();
builder.Services.AddScoped<IValidator<SaveHistoryRequest>, SaveHistoryRequestValidator>();

// Add response caching
builder.Services.AddResponseCaching();

// Add memory cache
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline
// Enable Swagger in both Development and Production
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Risk Calculator API v1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});

// Add global exception handling middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    await next();
});

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Enable CORS
app.UseCors("AllowFrontend");

// Enable response caching
app.UseResponseCaching();

app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting Risk Calculator API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
