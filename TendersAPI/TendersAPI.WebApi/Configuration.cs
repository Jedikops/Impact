using Microsoft.Extensions.Options;
using TendersApi.App.Handlers;
using TendersApi.App.Interfaces;
using TendersAPI.Infrastucture;
using TendersAPI.Infrastucture.Mapping;
using TendersAPI.Infrastucture.Settings;

namespace TendersAPI.WebApi
{
    public static class ConfigurationExtensions
    {
        public static TBuilder RegisterServices<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
        {
            builder.Services.Configure<TenderApiSettings>(builder.Configuration.GetSection("ApiSettings"));

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddTransient<GetTendersQueryHandler>();
            builder.Services.AddTransient<GetTenderByIdQueryHandler>();

            // Exception from the rule not to referece infra in WebApi (allowed by clean achritecture)
            builder.Services.AddSingleton<ITenderMapper, TenderMapper>();
            builder.Services.AddHttpClient<ITenderRepository, TenderRespository>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<TenderApiSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
            });

            return builder;

        }
    }
}
