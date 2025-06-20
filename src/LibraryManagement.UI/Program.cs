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

// 2. Rejestracja nazwanej instancji HttpClient do wywo³añ REST
builder.Services.AddHttpClient("ApiClient", client =>
{
    // base address Twojego API (CRUD ksi¹¿ek, autorów, etc.)
    client.BaseAddress = new Uri("https://localhost:7168/");
});

// 3. Rejestracja HttpClient dla wywo³añ wyszukiwania (gRPC-gateway lub osobny endpoint)
builder.Services.AddHttpClient("GrpcClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7089/");
    // je¿eli wyszukujesz pod innym portem / œcie¿k¹, dostosuj tutaj
});

// 4. Polityka autoryzacji tylko dla roli Admin
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});

// 5. Uwierzytelnianie cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.Cookie.Name = "LibraryAuth";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    });

var app = builder.Build();

// 6. Pipeline
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

// 7. Mapowanie Razor Pages
app.MapRazorPages();

app.Run();
