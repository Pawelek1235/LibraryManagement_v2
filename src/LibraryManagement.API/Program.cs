using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Infrastructure.Configuration;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repositories;
using LibraryManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Wczytanie ustawieñ JWT z appsettings.json
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));
var appSettings = builder.Configuration
    .GetSection("AppSettings")
    .Get<AppSettings>()!;

// 2. DbContext In-Memory
builder.Services.AddDbContext<LibraryDbContext>(opts =>
    opts.UseInMemoryDatabase("LibraryDb"));

// 3. Rejestracja UnitOfWork i generatora tokenów
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<JwtTokenGenerator>();

// 4. Uwierzytelnianie JWT-Bearer
var key = Encoding.UTF8.GetBytes(appSettings.JwtSecret!);
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = appSettings.JwtIssuer,
            ValidateAudience = true,
            ValidAudience = appSettings.JwtAudience,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuerSigningKey = true
        };
    });

// 5. Kontrolery
builder.Services.AddControllers();

// 6. Swagger/OpenAPI z obs³ug¹ Bearer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Library API", Version = "v1" });
    var sec = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "WprowadŸ token jako: Bearer {token}"
    };
    c.AddSecurityDefinition("Bearer", sec);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { sec, Array.Empty<string>() }
    });
});

var app = builder.Build();

// 7. Seedowanie danych
using var scope = app.Services.CreateScope();
var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

if (!(await uow.Members.GetAllAsync()).Any(m => m.Email == "admin@lib.com"))
{
    PasswordService.CreatePasswordHash("admin123", out var hash, out var salt);
    await uow.Members.AddAsync(new Member
    {
        FullName = "Administrator",
        Email = "admin@lib.com",
        MembershipDate = DateTime.UtcNow,
        PasswordHash = hash,
        PasswordSalt = salt,
        Role = "Admin"
    });
    await uow.SaveChangesAsync();
}

if (!(await uow.Authors.GetAllAsync()).Any())
{
    await uow.Authors.AddAsync(new Author
    {
        FullName = "Jan Kowalski",
        Bio = "Polski autor powieœci przygodowych."
    });
    await uow.Authors.AddAsync(new Author
    {
        FullName = "Anna Nowak",
        Bio = "Autorka ksi¹¿ek o programowaniu."
    });
    await uow.SaveChangesAsync();
}

// 8. Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
