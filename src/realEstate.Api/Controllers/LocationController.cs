using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using realEstate.Common.Domain.Model;
using realEstate.Common.Domain.Repositories;

namespace realEstate.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class LocationController : ControllerBase
    {
        private readonly ILocationRepository _locationRepository;

        public LocationController(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        [HttpGet("cities")]
        public async Task<List<string>> GetCities()
        {
           var cities= await _locationRepository.GetAsync();
           return cities.Select(x => x.Name).ToList();
        }

        [HttpGet("towns/{cityName}")]
        public async Task<List<string>> GetCities(string cityName)
        {
            var cities = await _locationRepository.GetAsync();
            var towns = cities.FirstOrDefault(x => x.Name == cityName)?.Towns;
            return towns.Select(x => x.Name).ToList();
        }
    }
}