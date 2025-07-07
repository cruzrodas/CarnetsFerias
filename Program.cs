using CarnetsdeFeria.Components;
using CarnetsdeFeria.Interfaces;
using CarnetsdeFeria.Models;
using CarnetsdeFeria.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
//Libreria de Mud Blazor
builder.Services.AddMudServices();


//builder.Services.AddDbContext<DBConexion>(); //conexion de database
builder.Services.AddDbContext<CarnetsFeriaContext>(Options =>
Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IParticipacion, SPaticipacion>();
builder.Services.AddTransient<IDocIdentificacion, SDocumentoIdentificacion>();
builder.Services.AddTransient<IFeria, SFerias>();
builder.Services.AddTransient<IEspacioParque, SEspacioParque>();
builder.Services.AddTransient<IAreasdeFeria, SAreasdeFeria>();
builder.Services.AddTransient<ICarnet, SCarnet>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
