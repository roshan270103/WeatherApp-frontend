using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Weatherfrontend;
using Weatherfrontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ✅ Register services
builder.Services.AddScoped<AuthState>();
builder.Services.AddScoped<FavoritesService>();
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// ✅ WeatherService needed by MainLayout
builder.Services.AddScoped<WeatherService>();

await builder.Build().RunAsync();
