using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using realEstate.Common.ExternalServices;
using realEstate.Common.InternalServices;
using realEstate.Common.Mapper;
using realEstate.Common.Mongo;
using realEstate.Worker.Workers;

namespace realEstate.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMongodb(hostContext.Configuration);
                    services.AddHttpClient<IHurriyetService, HurriyetService>(client =>
                        {
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
                            // client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; EN; rv:11.0) like Gecko");
                            client.BaseAddress = new Uri("https://www.hurriyetemlak.com/");
                        }
                        );
                    services.AddHttpClient<IEmlakJetService, EmlakJetService>(client =>
                        {
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
                            // client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; EN; rv:11.0) like Gecko");
                            client.BaseAddress = new Uri("https://www.emlakjet.com/");
                        }
                    );
                    services.AddHttpClient<ILocationService, LocationService>(client =>
                    {
                        client.BaseAddress = new Uri("https://il-ilce-rest-api.herokuapp.com/v1/");
                    });
                    services.AddSingleton<IInternalLocationService, InternalLocationService>();
                    services.AddSingleton<IListingDetailMapper, ListingDetailMapper>();

                    services.AddHostedService<HurriyetWorker>();
                    services.AddHostedService<EmlakJetWorker>();
                });
    }
}
