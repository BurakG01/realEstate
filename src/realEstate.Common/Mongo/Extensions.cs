using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using realEstate.Common.Config;
using realEstate.Common.Domain.Repositories;

namespace realEstate.Common.Mongo
{
    public static class Extensions
    {
        public static void AddMongodb(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<MongoOptions>(configuration.GetSection("Mongodb"));

            services.AddSingleton<MongoClient>(c =>
                       {
                           var options = c.GetService<IOptions<MongoOptions>>();
                           return new MongoClient(options.Value.ConnectionString);
                       });
            services.AddScoped(c =>
            {
                var options = c.GetService<IOptions<MongoOptions>>();
                var client = c.GetService<MongoClient>();

                return client.GetDatabase(options.Value.Database);
            });
            services.AddScoped<IRentingHouseRepository, RentingHouseRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();

        }
    }
}
