using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.Authorization;
using Blazorfirebase.Client.Authentication;
using Refit;
using Blazorfirebase.Client.Network;
using Blazorfirebase.Client.Services;
using Blazored.LocalStorage;

namespace Blazorfirebase.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddAuthorizationCore();

            builder.Services.AddTransient<AuthHeaderHandler>();
            
            builder.Services.AddRefitClient<IWeatherApiService>(new RefitSettings(new NewtonsoftJsonContentSerializer()))
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:6001/"))
                .AddHttpMessageHandler<AuthHeaderHandler>();
                
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            
            builder.Services.AddScoped<IWeatherService, WeatherService>();

            await builder.Build().RunAsync();
        }
    }
}
