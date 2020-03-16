using CommunityItaly.Services;
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
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter(level => true);
            });

            builder.Services.AddOptions<CosmosDbConnections>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("CosmosDbConnections").Bind(settings);
                });

            builder.Services.AddOptions<BlobStorageConnections>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                        var section = configuration.GetSection("BlobStorageConnections");
                        settings.ConnectionString = section.Get<string>();
                });
            builder.Services.UseService();
            builder.Services.AddScoped<IImageService>((sp) => new ImageService(sp.GetRequiredService<BlobStorageConnections>().ConnectionString));
        }
    }
}
