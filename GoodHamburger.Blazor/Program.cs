using GoodHamburger.Blazor;
using GoodHamburger.Blazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri("http://localhost:5000/") });
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<CardapioService>();

var app = builder.Build();

var authService = app.Services.GetRequiredService<AuthService>();
await authService.InitializeAsync();

await app.RunAsync();
