using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WellTrack.Api.Extensions;
using WellTrack.Services.Participation.Extensions;
using WellTrack.Services.Participation.Infrastructure.EF;
using WellTrack.Services.Participation.mappings;
using WellTrack.Services.Participation.Middleware;
using WellTrack.Services.Participation.Services;
using WellTrack.SharedKernel.Extensions;
using WellTrack.SharedKernel.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ── Configuration ─────────────────────────────────────────────────────────
builder.Configuration
	.AddJsonFile("appsettings.json")
	.AddJsonFile("appsettings.Local.json", optional: true)
	.AddEnvironmentVariables();

// ── Database ──────────────────────────────────────────────────────────────
builder.Services.AddDbContext<ParticipationDbContext>(opts =>
	opts.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection"),
		sql => sql.MigrationsAssembly("WellTrack.Services.Participation")
	));

// ── Auth ──────────────────────────────────────────────────────────────────
builder.Services.AddJwtAuthentication(builder.Configuration);

// ── Named HTTP client — Notification service ──────────────────────────────
builder.Services.AddHttpClient("NotificationClient",
    c => c.BaseAddress = new Uri(builder.Configuration["ServiceUrls:Notification"]!));

// ── HTTP client + engagement trigger ─────────────────────────────────────
builder.Services.AddHttpClient();
builder.Services.AddScoped<WellTrack.Services.Participation.Services.EngagementTrigger>();

// ── Business Logic ────────────────────────────────────────────────────────
builder.Services.AddParticipationModules();


// ── Infrastructure ────────────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddAutoMapper(cfg => { }, typeof(GoalMappingProfile).Assembly);

// ── API ───────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "WellTrack Participation Service", Version = "v1" });
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
	var db = scope.ServiceProvider.GetRequiredService<ParticipationDbContext>();
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
