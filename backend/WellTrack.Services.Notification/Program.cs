using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WellTrack.Services.Notification.Extensions;
using WellTrack.Services.Notification.Infrastructure.EF;
using WellTrack.Services.Notification.Mappings;
using WellTrack.Services.Notification.Middleware;
using WellTrack.Services.Notification.Services;
using WellTrack.SharedKernel.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Local.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddDbContext<NotificationDbContext>(opts =>
    opts.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("WellTrack.Services.Notification")
    ));

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddHttpClient("IdentityClient",
    c => c.BaseAddress = new Uri(builder.Configuration["ServiceUrls:Identity"]!));

builder.Services.AddNotificationModule();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddAutoMapper(cfg => { }, typeof(NotificationMappingProfile).Assembly);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WellTrack Notification Service", Version = "v1" });
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

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    db.Database.Migrate();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
