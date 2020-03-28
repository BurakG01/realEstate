using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
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
using realEstate.Common.ParsingModel;
using realEstate.Worker.Services;

namespace realEstate.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHurriyetService _hurriyetService;
        private readonly IServiceProvider _services;
        private readonly IInternalLocationService _internalLocationService;
        public Worker(ILogger<Worker> logger,
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
                    var rentingHouseRepository = scope.ServiceProvider.GetRequiredService<IRentingHouseRepository>();

                    var locations = await _internalLocationService.GetLocation();
                    foreach (var city in locations.Cities)
                    {
                        foreach (var town in city.Towns)
                        {
                            var townName = GetEnCharactersInString(town.Name.ToLower());
                            var response = await _hurriyetService.
                                GetAdvertsList($"{townName}-kiralik");
                            // todo : burada db den datayi cekip apiden gelenlerin arasinda olmayanlari silcez

                            foreach (var item in response)
                            {

                                var locationList = new List<LocationModel>()
                                        {
                                            new LocationModel()
                                            {
                                                Name = city.Name,Type = LocationType.City.ToString(),

                                            },
                                            new LocationModel(){Name = town.Name,Type = LocationType.Town.ToString()},
                                        };

                                var rentListing = new RentListing { Locations = new List<LocationModel>() };
                                int pos = item.Url.ToString().LastIndexOf("/", StringComparison.Ordinal) + 1;
                                rentListing.AdvertId = item.Url.ToString().Substring(pos, item.Url.ToString().Length - pos);
                                var detail = await _hurriyetService.GetAdvertDetail(item.Url.ToString());
                                rentListing.Name = item.Name;
                                rentListing.ShortDescription = detail.Offers.Description;
                                rentListing.Url = new Url() { Owner = Owners.HurriyetEmlak.ToString(), Link = item.Url.ToString() };
                                rentListing.Image = detail.Offers.Image.Select(x => x.ContentUrl.ToString()).ToList();
                                locationList.Add(new LocationModel() { Type = LocationType.Street.ToString(), Name = detail.Offers.ItemOfferedDetail.Address.StreetAddress });
                                rentListing.Locations = locationList;
                                rentListing.Owner = Owners.HurriyetEmlak.ToString();
                                rentListing.Price = detail.Offers.Price;
                                await rentingHouseRepository.UpsertRecord(rentListing);
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
    }
}
