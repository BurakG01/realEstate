using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using realEstate.Common.Domain.Model;
using realEstate.Common.Domain.Repositories;
using realEstate.Common.Enums;
using realEstate.Common.ExternalServices;
using realEstate.Worker.Services;

namespace realEstate.Worker.Workers
{
    public class HurriyetWorker : BackgroundService
    {
        private readonly ILogger<HurriyetWorker> _logger;
        private readonly IHurriyetService _hurriyetService;
        private readonly IServiceProvider _services;
        private readonly IInternalLocationService _internalLocationService;
        public HurriyetWorker(ILogger<HurriyetWorker> logger,
            IHurriyetService hurriyetService,
            IServiceProvider services,
            IInternalLocationService internalLocationService)
        {
            _logger = logger;
            _hurriyetService = hurriyetService;
            _services = services;
            _internalLocationService = internalLocationService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var deneme = await _hurriyetService.GetAdvertDetail("sd");
                    var rentingHouseRepository = scope.ServiceProvider.GetRequiredService<IRentingHouseRepository>();

                    var locations = await _internalLocationService.GetLocation();
                    foreach (var city in locations.Cities.OrderBy(x => x.Name))
                    {
                        foreach (var town in city.Towns.OrderBy(x=>x.Name))
                        {
                            var townName = GetEnCharactersInString(town.Name.ToLower());
                            foreach (var advertType in AdvertTypeList.AdvertTypeDictionary)
                            {
                               
                                var response = await _hurriyetService.
                                    GetAdvertsList($"{townName}-{advertType.Value}");
                                // todo : burada db den datayi cekip apiden gelenlerin arasinda olmayanlari silcez
                                foreach (var item in response)
                                {
                                    var itemUrl = item.Url.ToString();
                                    var rentListing = new Listing
                                    {
                                        City = new LocationModel { Name = city.Name, Id = city.Id },
                                        Town = new LocationModel { Name = town.Name, Id = town.Id },
                                        Name = item.Name,
                                        Url = itemUrl,
                                        OwnerSite = (int)Owners.HurriyetEmlak,
                                        AdvertType = advertType.Key,
                                        AdvertId = GetAdvertId(itemUrl)
                                   
                                    };
                                    var detail = await _hurriyetService.GetAdvertDetail(itemUrl);

                                    rentListing.Description=new Description
                                    {
                                        ShortDescription = detail.Offers.Description,
                                        FullDescription = detail.FullDescription,
                                        FullDescriptionInHtml = detail.FullDescriptionInHtml

                                    };
                                    rentListing.Images = detail.Offers.Image.Select(x => x.ContentUrl.ToString()).ToList();
                                    rentListing.Street = new LocationModel() { Name = detail.Offers.ItemOfferedDetail.Address.StreetAddress };
                                    rentListing.Price =new PriceModel() { Price = detail.Offers.Price, Currency = detail.Offers.PriceCurrency};
                               
                                    await rentingHouseRepository.UpsertRecord(rentListing);
                                }
                            }

                        }
                    }

                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(30 * 60 * 1000, stoppingToken);
            }
        }
        private string GetEnCharactersInString(string text)
        {
            StringBuilder sb = new StringBuilder(text);

            sb.Replace("ı", "i");
            sb.Replace("ü", "u");
            sb.Replace("ç", "c");
            sb.Replace("ö", "o");
            sb.Replace("ü", "u");
            sb.Replace("ğ", "g");
            sb.Replace("ş", "s");

            return sb.ToString();
        }
        private string GetAdvertId(string itemUrl)
        {
            int pos = itemUrl.LastIndexOf("/", StringComparison.Ordinal) + 1;
            return itemUrl.Substring(pos, itemUrl.Length - pos);
        }
    }
}
