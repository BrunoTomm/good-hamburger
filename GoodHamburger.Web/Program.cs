using GoodHamburger.Web;
using GoodHamburger.Web.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri("http://localhost:5050/") });
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<CardapioService>();
builder.Services.AddSingleton<CartService>();

var app = builder.Build();

var authService = app.Services.GetRequiredService<AuthService>();
await authService.InitializeAsync();

await app.RunAsync();