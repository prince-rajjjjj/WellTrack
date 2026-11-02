using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WellTrack.Services.Compliance.Extensions;
using WellTrack.Services.Compliance.Extensions;
using WellTrack.Services.Compliance.Infrastructure.EF;
using WellTrack.Services.Compliance.Mappings;
using WellTrack.Services.Compliance.Middleware;
using WellTrack.Services.Compliance.Services;
using WellTrack.SharedKernel.Extensions;
using WellTrack.SharedKernel.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ── Configuration ─────────────────────────────────────────────────────────
builder.Configuration
	.AddJsonFile("appsettings.json")
	.AddJsonFile("appsettings.Local.json", optional: true)
	.AddEnvironmentVariables();

// ── Database ──────────────────────────────────────────────────────────────
builder.Services.AddDbContext<ComplianceDbContext>(opts =>
	opts.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection"),
		sql => sql.MigrationsAssembly("WellTrack.Services.Compliance")
	));

// ── Auth ──────────────────────────────────────────────────────────────────
builder.Services.AddJwtAuthentication(builder.Configuration);

// ── Business Logic ────────────────────────────────────────────────────────
builder.Services.AddComplianceModule();

// ── Infrastructure ────────────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddAutoMapper(cfg => { }, typeof(AutoMapperProfiles).Assembly);

// ── API ───────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "WellTrack Compliance Service", Version = "v1" });
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "JWT Authorization header using the Bearer scheme",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
	});
	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{{
		new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
		Array.Empty<string>()
	}});
});
//-----notification client ------
builder.Services.AddNotificationClient(builder.Configuration);
builder.Services.AddIdentityClient(builder.Configuration);

var app = builder.Build();

// ── Auto-migrate on startup (dev only) ───────────────────────────────────
if (app.Environment.IsDevelopment())
{
	using var scope = app.Services.CreateScope();
	var db = scope.ServiceProvider.GetRequiredService<ComplianceDbContext>();
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
