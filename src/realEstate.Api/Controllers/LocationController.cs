using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using realEstate.Api.FilterModel;
using realEstate.Common.Domain.Model;
using realEstate.Common.Domain.Repositories;
using realEstate.Common.ExternalServices;

namespace realEstate.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class LocationController : ControllerBase
    {
        private readonly ILocationRepository _locationRepository;
        private readonly INeighborhoodRepository _neighborhoodRepository;
        private readonly ILocationService _locationService;

        public LocationController(ILocationRepository locationRepository,
            INeighborhoodRepository neighborhoodRepository,
            ILocationService locationService)
        {
            _locationRepository = locationRepository;
            _neighborhoodRepository = neighborhoodRepository;
            _locationService = locationService;
        }

        [HttpGet("cities")]
        public async Task<List<string>> GetCities()
        {
            var cities = await _locationRepository.GetAsync();
            return cities.Select(x => x.Name).ToList();
        }

        [HttpGet("towns/{cityName}")]
        public async Task<List<dynamic>> GetCities(string cityName)
        {
            var cities = await _locationRepository.GetAsync();
            var towns = cities.FirstOrDefault(x => x.Name == cityName)?.Towns;
            return towns.Select(x => new
            {
                Name = x.Name,
                Code = x.Id
            }).ToList<dynamic>();
        }


        [HttpPost("neighborhoods")]
        public async Task<List<dynamic>> GetNeighborhoods([FromBody] NeighborhoodFilterModel filter)
        {
            var @return = new List<dynamic>();
            foreach (var id in filter.Towns)
            {
                var response = await _locationService.GetNeighborhoods(id);
                var returnList = response.NeighborhoodList.Select(x => new
                {
                    Name = x.Name,
                    District = x.District,
                    Id = x.Name.Substring(0, x.Name.IndexOf("mah", StringComparison.Ordinal)).TrimEnd()
                }).ToList<dynamic>();
                @return.AddRange(returnList);
            }
            return @return;

        }
    }
}