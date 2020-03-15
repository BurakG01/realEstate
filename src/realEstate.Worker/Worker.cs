using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using realEstate.Common.Domain.Model;
using realEstate.Common.Domain.Repositories;
using realEstate.Common.ExternalServices;
using realEstate.Common.ParsingModel;

namespace realEstate.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHurriyetService _hurriyetService;
        private readonly IServiceProvider _services;
        public Worker(ILogger<Worker> logger, IHurriyetService hurriyetService, IServiceProvider services)
        {
            _logger = logger;
            _hurriyetService = hurriyetService;
            _services = services;
        }



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                using (var scope = _services.CreateScope())
                {
                    var rentingHouseRepository = scope.ServiceProvider.GetRequiredService<IRentingHouseRepository>();
                    var response = await _hurriyetService.GetAdvertsList($"cukurova-kiralik?districts=cukurova-guzelyali");


                    foreach (var item in response)
                    {
                        var rentingHouseModel = new RentingHouse();
                        rentingHouseModel.Offers = new Offers();


                        int pos = item.Url.ToString().LastIndexOf("/", StringComparison.Ordinal) + 1;
                        rentingHouseModel.AdvertId = item.Url.ToString().Substring(pos, item.Url.ToString().Length - pos);
                        var detail = await _hurriyetService.GetAdvertDetail(item.Url.ToString());
                        rentingHouseModel.Name = item.Name;
                        rentingHouseModel.Offers = item.Offers;
                        rentingHouseModel.Description = detail.Offers.Description;
                        rentingHouseModel.PathList=new List<string>(){"pathDenemesi"};
                        rentingHouseModel.Image = detail.Offers.Image.Select(x => x.ContentUrl.ToString()).ToList();
                            await rentingHouseRepository.UpsertRecord(rentingHouseModel);
                      
                    
                    }
                }



                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(60 * 1000, stoppingToken);
            }
        }
    }
}
