using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using realEstate.Common.ExternalServices;

namespace realEstate.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHurriyetService _hurriyetService;

        public Worker(ILogger<Worker> logger, IHurriyetService hurriyetService)
        {
            _logger = logger;
            _hurriyetService = hurriyetService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var response = await _hurriyetService.GetAdvertsList("cukurova-kiralik?districts=cukurova-guzelyali");

                foreach (var item in response)
                {
                    // todo : create new model to merge for  data of items and second request data 
                    // todo : get data what we need from item then map to you already created new model above.
                    // todo : create new method in hurriyetservice 
                    // todo : make another request for every "url " of item .
                    // todo : get  other data such as descriptions from second request then map to you already created new model above.                    
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(60*1000, stoppingToken);
            }
        }
    }
}
