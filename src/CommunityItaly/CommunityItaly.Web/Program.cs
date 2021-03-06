using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CommunityItaly.Web.Services;
using MatBlazor;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using FluentValidation;

namespace CommunityItaly.Web
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("app");

			builder.Services.AddBaseAddressHttpClient();
			builder.Services.AddTransient<IHttpServices, HttpServices>();
			builder.Services.AddValidatorsFromAssemblyContaining<Program>();
			builder.Services.AddMatToaster(config =>
			{
				config.Position = MatToastPosition.TopRight;
				config.PreventDuplicates = true;
				config.NewestOnTop = true;
				config.ShowCloseButton = true;
				config.MaximumOpacity = 95;
				config.VisibleStateDuration = 3000;
			});

			await builder.Build().UseLocalTimeZone().RunAsync();
		}
	}
}
