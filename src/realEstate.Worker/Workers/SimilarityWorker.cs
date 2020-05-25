using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using F23.StringSimilarity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using realEstate.Common.Domain.Repositories;
using realEstate.Common.Enums;
using realEstate.Common.InternalServices;

namespace realEstate.Worker.Workers
{
    public class SimilarityWorker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly IInternalLocationService _internalLocationService;
        public SimilarityWorker(IServiceProvider services, IInternalLocationService internalLocationService)
        {
            _services = services;
            _internalLocationService = internalLocationService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();
                var cities = await _internalLocationService.GetCities();
                var listingRepository = scope.ServiceProvider.GetRequiredService<IListingRepository>();
                foreach (var city in cities)
                {
                    foreach (var town in city.Towns)
                    {
                        var listings = listingRepository.GetByFilter(x => x.City.Name == city.Name &&
                                                                        x.Town.Name == town.Name).ToList();

                        var emlakJetListings = listings.Where(x => x.OwnerSite == (int)Owners.EmlakJet
                                                                   && !string.IsNullOrEmpty(x.FullDescription)).ToList();
                        var hurriyetListings = listings.Where(x => x.OwnerSite == (int)Owners.HurriyetEmlak
                                                                   && !string.IsNullOrEmpty(x.FullDescription)).ToList();


                        foreach (var hurriyetListing in hurriyetListings)
                        {
                            
                                var roomNumber = !string.IsNullOrEmpty(hurriyetListing.RoomNumber) ?
                                    hurriyetListing.RoomNumber.Replace(" ", string.Empty):"";
                            
                         
                            var samePropertyListings = emlakJetListings.Where(x =>
                                x.RoomNumber.Trim() == roomNumber &&
                                x.Street.Name==hurriyetListing.Street.Name&&
                                x.AdvertStatus == hurriyetListing.AdvertStatus);

                            foreach (var samePropertyListing in samePropertyListings)
                            {
                                var similarityMeasure = GetSimilarity(hurriyetListing.FullDescription,
                                    samePropertyListing.FullDescription);
                                if (similarityMeasure > 0.9)
                                {
                                    var newSku = $"RE{hurriyetListing.AdvertId}{samePropertyListing.AdvertId}";
                                    await listingRepository.UpdateMany(
                                        new List<ObjectId>() { hurriyetListing.Id, samePropertyListing.Id }, newSku);
                                    break;

                                } 
                            }

                        }

                    }
                }
            }
        }

        private double GetSimilarity(string firstDescription, string secondDescription)
        {
            var jw = new JaroWinkler();

            return jw.Similarity(firstDescription,secondDescription);
        }
    }
}
