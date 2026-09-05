using CleaningSuite.Application;
using CleaningSuite.Infrastructure.Persistence;
using JasperFx;
using Microsoft.OpenApi.Models;

// Load .env from repo root before config builds (dev convenience, DotNetEnv is a no-op if absent).
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cleaning Suite API", Version = "v1" });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings__DefaultConnection missing");

var autoCreate = builder.Configuration.GetValue<AutoCreate?>("Marten:AutoCreateSchemaMode")
    ?? AutoCreate.CreateOrUpdate;

builder.Services.AddCleaningMarten(connectionString, autoCreate);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:3000"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("SiteOrigins", policy =>
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Health check: liveness only, no DB dependency.
app.MapGet("/healthz", () => Results.Ok(new { status = "ok", utc = DateTime.UtcNow }));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("SiteOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
