using KironTest.Services.Cache;
using KironTest.Services.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using KironTest.Repositories.NavigationRepositories;
using KironTest.Repositories.UserRepositories;
using KironTest.Repositories.BankHolidayRepositories;

var builder = WebApplication.CreateBuilder(args);

// Add MemoryCache
builder.Services.AddMemoryCache();

// Register the caching service
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

// Add services to the container.
builder.Services.AddControllers();

// Add DBConnectionManager as a singleton
builder.Services.AddSingleton<DBConnectionManagerService>();

// Enable Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "KironTest API",
        Version = "v1",
        Description = "A test API for Kiron"
    });
});

// Register ApplicationDbContext with SQL Server connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<NavigationRepository>();
builder.Services.AddScoped<BankHolidayRepository>();
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

var app = builder.Build();

app.UseCors("AllowAllOrigins");

// Configure middleware
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "KironTest API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
