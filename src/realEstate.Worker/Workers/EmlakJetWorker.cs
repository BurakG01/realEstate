using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using realEstate.Common.ExternalServices;
using realEstate.Worker.Services;

namespace realEstate.Worker.Workers
{
    public class EmlakJetWorker : BackgroundService
    {
        private readonly ILogger<EmlakJetWorker> _logger;
        private readonly IEmlakJetService _emlakJetService;
        private readonly IServiceProvider _services;
        private readonly IInternalLocationService _internalLocationService;

        public EmlakJetWorker(ILogger<EmlakJetWorker> logger, IEmlakJetService emlakJetService, IServiceProvider services, IInternalLocationService internalLocationService)
        {
            _logger = logger;
            _emlakJetService = emlakJetService;
            _services = services;
            _internalLocationService = internalLocationService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var response = await _emlakJetService.GetAdvertList("kiralik-konut/adana/");

        }
    }
}
