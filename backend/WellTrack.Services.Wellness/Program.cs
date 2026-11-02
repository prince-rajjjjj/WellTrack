using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WellTrack.Services.Wellness.Extensions;
using WellTrack.Services.Wellness.Extensions;
using WellTrack.Services.Wellness.Infrastructure.EF;
using WellTrack.Services.Wellness.mappings;
using WellTrack.Services.Wellness.Middleware;
using WellTrack.Services.Wellness.Services;
using WellTrack.SharedKernel.Extensions;
using WellTrack.SharedKernel.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ── Configuration ─────────────────────────────────────────────────────────
builder.Configuration
	.AddJsonFile("appsettings.json")
	.AddJsonFile("appsettings.Local.json", optional: true)
	.AddEnvironmentVariables();

// ── Database ──────────────────────────────────────────────────────────────
builder.Services.AddDbContext<ProgramDbContext>(opts =>
	opts.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection"),
		sql => sql.MigrationsAssembly("WellTrack.Services.Wellness")
	));

// ── Auth ──────────────────────────────────────────────────────────────────
builder.Services.AddJwtAuthentication(builder.Configuration);

// ── HTTP clients (used by EngagementTrigger) ──────────────────────────────
builder.Services.AddHttpClient();
builder.Services.AddScoped<WellTrack.Services.Wellness.Services.EngagementTrigger>();
builder.Services.AddScoped<WellTrack.Services.Wellness.Services.ParticipationTrigger>();

// ── Business Logic ────────────────────────────────────────────────────────
builder.Services.AddWellnessModule();

// ── Infrastructure ────────────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddAutoMapper(cfg => { }, typeof(WellnessMappingProfile).Assembly);

// ── CORS ──────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontend", policy =>
		policy.WithOrigins("http://localhost:4200")
			  .AllowAnyHeader()
			  .AllowAnyMethod()
			  .AllowCredentials());
});

// ── API ───────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "WellTrack Wellness Service", Version = "v1" });
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
		new OpenApiSecurityScheme
		{
			Reference = new OpenApiReference
			{
				Type = ReferenceType.SecurityScheme,
				Id = "Bearer"
			}
		},
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
	var db = scope.ServiceProvider.GetRequiredService<ProgramDbContext>();
	db.Database.Migrate();

	app.UseSwagger();
	app.UseSwaggerUI();
}

// ── Middleware pipeline ───────────────────────────────────────────────────
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
