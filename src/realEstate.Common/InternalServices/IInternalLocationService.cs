using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using realEstate.Common.Domain.Model;
using realEstate.Common.Domain.Repositories;
using realEstate.Common.ExternalServices;

namespace realEstate.Common.InternalServices
{
    public interface IInternalLocationService
    {
        Task<List<City>> GetCities();
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
                        //foreach (var town in towns.TownList.OrderBy(x=>x.Name))
                        //{
                        //    var newTown = new Town
                        //    {
                        //        Name = town.Name,
                        //        Id = town.Id,
                        //        Districts = new List<District>()
                        //    };
                        //    var districts = await _locationService.GetDistricts(town.Id);

                        //    foreach (var district in districts.DistrictList.OrderBy(x=>x.Name))
                        //    {
                        //        var newDistrict = new District()
                        //        {
                        //            Name = district.Name,
                        //            Id = district.Id,
                        //            Neighborhoods = new List<Neighborhood>()
                        //        };
                        //        var neighborhoods = await _locationService.GetNeighborhoods(district.Id);
                        //        newDistrict.Neighborhoods = neighborhoods.NeighborhoodList.Select(x => new Neighborhood
                        //        {
                        //            Name = x.Name,
                        //            Id = x.Id
                        //        }).ToList();
                        //        newTown.Districts.Add(newDistrict);
                        //    }

                        //    newCity.Towns.Add(newTown);
                        //}

                        await locationRepository.AddAsync(newCity);
                        cityList.Add(newCity);
                    }

                    return cityList;
                }
            }

        }
    }
}
