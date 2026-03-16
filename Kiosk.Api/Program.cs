using Kiosk.Data;
using Kiosk.Domain.Services;
using Kiosk.Domain.Interfaces;
using Kiosk.Domain.Mapping;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<KioskDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Register services
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IKioskService, KioskService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IAuthService, AuthService>();
// Avoid circular reference issues in JSON serialization
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Kiosk API";
    config.Version = "v1";
    config.Description = "API for managing customers, kiosks, and devices";

    // Add JWT Bearer security definition
    config.AddSecurity("Bearer", new NSwag.OpenApiSecurityScheme
    {
        Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = NSwag.OpenApiSecurityApiKeyLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
    });

    // Make JWT Bearer the default security scheme
    config.OperationProcessors.Add(new OperationSecurityScopeProcessor("Bearer"));
});

var app = builder.Build();

// Apply migrations and seed data idempotently
// I´m aware that this is not the best approach, I included it just for the technical test, in a real project I would use a more robust solution like a separate migration tool or scripts.
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<KioskDbContext>();
        await db.Database.MigrateAsync();

        var seedEnabled = app.Configuration.GetValue<bool>("Seed:Enabled", true);
        if (seedEnabled)
        {
            await KioskDbContextSeed.SeedAsync(db, seedEnabled);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        throw;
    }
}

// Only enable Swagger in development environment
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
