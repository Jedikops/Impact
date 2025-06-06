using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Polly;
using Serilog;
using TendersApi.App.Handlers;
using TendersApi.App.Interfaces;
using TendersApi.Infrastucture.Mapping;
using TendersApi.Infrastucture.Repositories;
using TendersApi.Infrastucture.Settings;
using TendersAPI.WebApi.Middlewares;

namespace TendersApi.WebApi
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
            builder.Services.AddHttpClient<TenderRespository>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<TenderApiSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
            }).AddPolicyHandler(
                Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMinutes(1))
                .WrapAsync(Policy.Handle<HttpRequestException>().RetryAsync(3))
            );


            builder.Services.AddScoped<ITenderRepository, CachedTenderRepository>(sp =>
            {
                var inner = sp.GetRequiredService<TenderRespository>();
                var cache = sp.GetRequiredService<IDistributedCache>();

                return new CachedTenderRepository(cache, inner);

            });

            return builder;

        }
    }
}
