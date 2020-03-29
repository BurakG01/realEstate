using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using realEstate.Common.Domain.Model;
using realEstate.Common.Domain.Repositories;
using realEstate.Common.ExternalServices;

namespace realEstate.Worker.Services
{
    public interface IInternalLocationService
    {
        Task<Location> GetLocation();
    }

    public class InternalLocationService : IInternalLocationService
    {
        private readonly ILocationService _locationService;
        private readonly IServiceProvider _services;

        public InternalLocationService(
            ILocationService locationService, IServiceProvider services)
        {
            _locationService = locationService;
            _services = services;
        }

        public async Task<Location> GetLocation()
        {
            using (var scope = _services.CreateScope())
            {
                var locationRepository = scope.ServiceProvider.GetRequiredService<ILocationRepository>();
                var location = await locationRepository.GetAsync();
                if (location != null)
                {
                    return location;
                }
                else
                {
                    var cities = await _locationService.GetCities();
                    var newLocation = new Location() { Cities = new List<City>() };

                    foreach (var city in cities.Data)
                    {
                        var newCity = new City() { Name = city.Name, Id = city.Id, Towns = new List<Town>() };
                        var towns = await _locationService.GetTowns(city.Id);
                        newCity.Towns = towns.TownList.Select(x => new Town
                        {
                            Id = x.Id,
                            Name = x.Name
                        }).ToList();
                        newLocation.Cities.Add(newCity);
                    }

                    await locationRepository.AddAsync(newLocation);
                    return newLocation;
                }
            }

        }
    }
}
