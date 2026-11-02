using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WellTrack.Services.Engagement.Extensions;
using WellTrack.Services.Engagement.Extensions;
using WellTrack.Services.Engagement.Infrastructure.EF;
using WellTrack.Services.Engagement.Mapping;
using WellTrack.Services.Engagement.Middleware;
using WellTrack.Services.Engagement.Services;
using WellTrack.SharedKernel.Extensions;
using WellTrack.SharedKernel.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ── Configuration ─────────────────────────────────────────────────────────
builder.Configuration
	.AddJsonFile("appsettings.json")
	.AddJsonFile("appsettings.Local.json", optional: true)
	.AddEnvironmentVariables();

// ── Database ──────────────────────────────────────────────────────────────
builder.Services.AddDbContext<EngagementDbContext>(opts =>
	opts.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection"),
		sql => sql.MigrationsAssembly("WellTrack.Services.Engagement")
	));

// ── Auth ──────────────────────────────────────────────────────────────────
builder.Services.AddJwtAuthentication(builder.Configuration);

// ── Business Logic ────────────────────────────────────────────────────────
builder.Services.AddEngagementModule(builder.Configuration);

// ── Infrastructure ────────────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

// ── API ───────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "WellTrack Engagement Service", Version = "v1" });
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
	var db = scope.ServiceProvider.GetRequiredService<EngagementDbContext>();
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
