using Blazorise;
using Blazorise.Bulma;
using Blazorise.Icons.FontAwesome;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using AspNetMonsters.Blazor.Geolocation;

namespace Reststops.Presentation.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
           
            builder.Services
                .AddScoped<LocationService>()
                .AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
                .AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true;
                })
                .AddBulmaProviders()
                .AddFontAwesomeIcons();

            var host = builder.Build();

            host.Services
                .UseBulmaProviders()
                .UseFontAwesomeIcons();

            await host.RunAsync();
        }
    }
}
