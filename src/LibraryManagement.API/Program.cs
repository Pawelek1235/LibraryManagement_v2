using LibraryManagement.API.Middleware;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Infrastructure.Configuration;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repositories;
using LibraryManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

builder.Services.AddDbContext<LibraryDbContext>(opts =>
    opts.UseInMemoryDatabase("LibraryDb"));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<JwtTokenGenerator>();

builder.Services.AddControllers();

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
        Description = "Wprowadü token jako: Bearer {token}"
    };
    c.AddSecurityDefinition("Bearer", sec);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { sec, Array.Empty<string>() }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
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
}

using (var scope = app.Services.CreateScope())
{
    var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    if (!(await uow.Authors.GetAllAsync()).Any())
    {
        await uow.Authors.AddAsync(new Author
        {
            FullName = "Jan Kowalski",
            Bio = "Polski autor powieúci przygodowych."
        });
        await uow.Authors.AddAsync(new Author
        {
            FullName = "Anna Nowak",
            Bio = "Autorka ksiπøek o programowaniu."
        });
        await uow.SaveChangesAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API v1"));
}

app.UseHttpsRedirection();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<JwtMiddleware>();
app.MapControllers();
app.Run();
