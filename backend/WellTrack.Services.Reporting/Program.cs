using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WellTrack.Services.Reporting.Extensions;
using WellTrack.Services.Reporting.Infrastructure.EF;
using WellTrack.Services.Reporting.Extensions;
using WellTrack.Services.Reporting.Middleware;
using WellTrack.Services.Reporting.Services;
using WellTrack.SharedKernel.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ── Configuration ─────────────────────────────────────────────────────────
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Local.json", optional: true)
    .AddEnvironmentVariables();

// ── Database ──────────────────────────────────────────────────────────────
builder.Services.AddDbContext<ReportingDbContext>(opts =>
    opts.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("WellTrack.Services.Reporting")
    ));

// ── Auth ──────────────────────────────────────────────────────────────────
builder.Services.AddJwtAuthentication(builder.Configuration);

// ── Named HTTP clients (downstream services) ──────────────────────────────
var urls = builder.Configuration.GetSection("ServiceUrls");

builder.Services.AddHttpClient("IdentityClient",
    c => c.BaseAddress = new Uri(urls["Identity"]!));

builder.Services.AddHttpClient("WellnessClient",
    c => c.BaseAddress = new Uri(urls["Wellness"]!));

builder.Services.AddHttpClient("ParticipationClient",
    c => c.BaseAddress = new Uri(urls["Participation"]!));

builder.Services.AddHttpClient("EngagementClient",
    c => c.BaseAddress = new Uri(urls["Engagement"]!));

builder.Services.AddHttpClient("ComplianceClient",
    c => c.BaseAddress = new Uri(urls["Compliance"]!));

// ── Business Logic ────────────────────────────────────────────────────────
builder.Services.AddReportingModule();

// ── Infrastructure ────────────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// ── API ───────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WellTrack Reporting Service", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name        = "Authorization",
        In          = ParameterLocation.Header,
        Type        = SecuritySchemeType.ApiKey,
        Scheme      = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {{
        new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
        Array.Empty<string>()
    }});
});

var app = builder.Build();

// ── Auto-migrate on startup (dev only) ───────────────────────────────────
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ReportingDbContext>();
    db.Database.Migrate();

    app.UseSwagger();
    app.UseSwaggerUI();
}

// ── Middleware pipeline ───────────────────────────────────────────────────
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
