using CommunityItaly.EF;
using CommunityItaly.EF.Settings;
using CommunityItaly.Services;
using CommunityItaly.Services.Settings;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(CommunityItaly.Server.Startup))]
namespace CommunityItaly.Server
{
	public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            ConfigureServices(builder.Services);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ReadOptions(services);
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter(level => true);
            });
            services.UseService();
            services.UseDatabase();
            services.AddScoped<IImageService, ImageService>();
        }

        // https://docs.microsoft.com/bs-latn-ba/azure/azure-functions/functions-dotnet-dependency-injection#working-with-options-and-settings
        public void ReadOptions(IServiceCollection services)
        {
            services.AddOptions<CosmosDbConnections>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("CosmosDbConnections").Bind(settings);
                });

            services.AddOptions<BlobStorageConnections>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("BlobStorageConnections").Bind(settings);
                });

            services.AddOptions<SendGridConnections>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("SendGridConnections").Bind(settings);
                });

            services.AddOptions<AdminConfiguration>()
               .Configure<IConfiguration>((settings, configuration) =>
               {
                   configuration.GetSection("AdminConfiguration").Bind(settings);
               });
        }
    }
}