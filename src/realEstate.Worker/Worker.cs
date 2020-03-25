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
        public Worker(ILogger<Worker> logger, IHurriyetService hurriyetService, IServiceProvider services, IInternalLocationService internalLocationService)
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

                            foreach (var item in response)
                            {

                                var locationList = new List<LocationModel>()
                                        {
                                            new LocationModel(){Name = city.Name,Type = "City"},
                                            new LocationModel(){Name = town.Name,Type = "Town"},
                                        };

                                var rentingHouseModel = new RentingHouse { Offers = new Offers() };
                                int pos = item.Url.ToString().LastIndexOf("/", StringComparison.Ordinal) + 1;
                                rentingHouseModel.AdvertId = item.Url.ToString().Substring(pos, item.Url.ToString().Length - pos);
                                var detail = await _hurriyetService.GetAdvertDetail(item.Url.ToString());
                                rentingHouseModel.Name = item.Name;
                                rentingHouseModel.Offers = item.Offers;
                                rentingHouseModel.Description = detail.Offers.Description;
                                rentingHouseModel.PathList = new List<string>() { item.Url.ToString() };
                                rentingHouseModel.Image = detail.Offers.Image.Select(x => x.ContentUrl.ToString()).ToList();
                                locationList.Add(new LocationModel() { Type = "Street", Name = detail.Offers.ItemOfferedDetail.Address.StreetAddress });
                                rentingHouseModel.Locations = locationList;
                                await rentingHouseRepository.UpsertRecord(rentingHouseModel);
                            }

                        }
                    }

                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(15 * 60 * 1000, stoppingToken);
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
