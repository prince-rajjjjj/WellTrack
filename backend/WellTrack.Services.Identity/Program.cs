using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WellTrack.Services.Identity.Extensions;
using WellTrack.Services.Identity.Mappings;
using WellTrack.Services.Identity.Middleware;
using WellTrack.Services.Identity.Services;
using WellTrack.SharedKernel.Extensions;
using WellTrack.SharedKernel.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ── Configuration ─────────────────────────────────────────────────────────
builder.Configuration
	.AddJsonFile("appsettings.json")
	.AddJsonFile("appsettings.Local.json", optional: true)
	.AddEnvironmentVariables();

// ── Database ──────────────────────────────────────────────────────────────
builder.Services.AddDbContext<WellTrack.Services.Identity.Infrastructure.EF.IdentityDbContext>(opts =>
	opts.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection"),
		sql => sql.MigrationsAssembly("WellTrack.Services.Identity")
	));

// ── Auth ──────────────────────────────────────────────────────────────────
builder.Services.AddJwtAuthentication(builder.Configuration);

// ── Business Logic ────────────────────────────────────────────────────────
builder.Services.AddIdentityModule();

// ── Infrastructure ────────────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddAutoMapper(cfg => { }, typeof(IdentityMappingProfile).Assembly);

// ── API ───────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "WellTrack Identity Service", Version = "v1" });
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

// ── Notification part ───────────────────────────────────────────────────────────────────
builder.Services.AddNotificationClient(builder.Configuration);
builder.Services.AddIdentityClient(builder.Configuration);

var app = builder.Build();

// ── Auto-migrate on startup (dev only) ───────────────────────────────────
if (app.Environment.IsDevelopment())
{
	using var scope = app.Services.CreateScope();
	var db = scope.ServiceProvider
		.GetRequiredService<WellTrack.Services.Identity.Infrastructure.EF.IdentityDbContext>();
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
