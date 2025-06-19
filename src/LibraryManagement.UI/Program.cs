using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;

var builder = WebApplication.CreateBuilder(args);

// 1. Dodaj Razor Pages i zabezpiecz folder /Admin
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
});

// 2. Rejestracja nazwanej instancji HttpClient do wywo³añ REST/gRPC
builder.Services.AddHttpClient("ApiClient", client =>
{
    // Ustaw base address na adres Twojego API (sprawdŸ port w konsoli API!)
    client.BaseAddress = new Uri("https://localhost:7168/");
});

// 3. Konfiguracja polityki autoryzacji (role)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});

// 4. Uwierzytelnianie cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.Cookie.Name = "LibraryAuth";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    });

var app = builder.Build();

// 5. Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// najpierw uwierzytelnianie, potem autoryzacja
app.UseAuthentication();
app.UseAuthorization();

// 6. Mapowanie Razor Pages
app.MapRazorPages();

app.Run();
