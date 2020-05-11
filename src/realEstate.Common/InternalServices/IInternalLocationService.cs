using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using realEstate.Common.Domain.Model;
using realEstate.Common.Domain.Repositories;
using realEstate.Common.ExternalServices;
using realEstate.Common.ParsingModel;
using Town = realEstate.Common.Domain.Model.Town;

namespace realEstate.Common.InternalServices
{
    public interface IInternalLocationService
    {
        Task<List<City>> GetCities();
        Task GetNeighborhoods();

    }

    public class InternalLocationService : IInternalLocationService
    {
        private readonly ILocationService _locationService;
        private readonly IServiceProvider _services;
      

        public InternalLocationService(
            ILocationService locationService, IServiceProvider services
           )
        {
            _locationService = locationService;
            _services = services;
          
        }

        public async Task<List<City>> GetCities()
        {
            using (var scope = _services.CreateScope())
            {
                var locationRepository = scope.ServiceProvider.GetRequiredService<ILocationRepository>();
                var location = await locationRepository.GetAsync();
                if (location.Any())
                {
                    return location;
                }
                else
                {
                    var cityList = new List<City>();
                    var cities = await _locationService.GetCities();

                    foreach (var city in cities.Data.OrderBy(x => x.Name))
                    {
                        var newCity = new City()
                        {

                            Id = city.Id,
                            Name = city.Name,
                            Towns = new List<Town>()

                        };
                        var towns = await _locationService.GetTowns(city.Id);
                        newCity.Towns = towns.TownList.Select(x => new Town
                        {
                            Name = x.Name,
                            Id = x.Id

                        }).OrderBy(x => x.Name).ToList();

                        await locationRepository.AddAsync(newCity);
                        cityList.Add(newCity);
                    }

                    return cityList;
                }
            }

        }

        public async Task GetNeighborhoods()
        {
            using (var scope = _services.CreateScope())
            {
                var _neighborhoodRepository = scope.ServiceProvider.GetRequiredService<INeighborhoodRepository>();
                var isExist = _neighborhoodRepository.GetByFilter(_ => true);
                if (!isExist.Any())
                {

                    var neighborhoods = new List<NeighborhoodData>();
                    var skip = 0;

                    while (true)
                    {
                        var response = await _locationService.GetAllNeighborhoods(skip);
                        if (response.NeighborhoodList.Count < 100)
                        {
                            break;
                        }
                        neighborhoods.AddRange(response.NeighborhoodList);
                        skip++;
                    }

                    await _neighborhoodRepository.AddAsync(neighborhoods);
                }
            }

        }
    }
}
