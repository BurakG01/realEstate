using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using realEstate.Common.ParsingModel;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace realEstate.Common.ExternalServices
{
    public interface ILocationService
    {
        Task<Cities> GetCities();
        Task<Towns> GetTowns(string cityId);
        Task<Neighborhoods> GetAllNeighborhoods(int skip);
        Task<Districts> GetDistricts(string townId);
        Task<Neighborhoods> GetNeighborhoods(string districtId);
    }

    public class LocationService : ILocationService
    {
        private readonly HttpClient _httpClient;

        public LocationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Cities> GetCities()
        {
            var response = await _httpClient.GetAsync($"cities");
            if (!response.IsSuccessStatusCode)
            {
                //todo : throw exception
            }
            var result = await response.Content.ReadAsStringAsync();
            var cities = JsonConvert.DeserializeObject<Cities>(result);
            if (!cities.Status)
            {
                // todo : throw an exception
            }

            return cities;
        }
        public async Task<Towns> GetTowns(string cityId)
        {
            var response = await _httpClient.GetAsync($"cities/{cityId}/towns");
            if (!response.IsSuccessStatusCode)
            {
                //todo : throw exception
            }
            var result = await response.Content.ReadAsStringAsync();
            var towns = JsonConvert.DeserializeObject<Towns>(result);

            return towns;
        }

        public async Task<Districts> GetDistricts(string townId)
        {
            var response = await _httpClient.GetAsync($"towns/{townId}/districts");
            if (!response.IsSuccessStatusCode)
            {
                //todo : throw exception
            }
            var result = await response.Content.ReadAsStringAsync();
            var districts = JsonConvert.DeserializeObject<Districts>(result);

            return districts;
        }

        public async Task<Neighborhoods> GetNeighborhoods(string districtId)
        {
            var response = await _httpClient.GetAsync($"towns/{districtId}/neighborhoods");
            if (!response.IsSuccessStatusCode)
            {
                //todo : throw exception
            }
            var result = await response.Content.ReadAsStringAsync();
            var neighborhoods = JsonConvert.DeserializeObject<Neighborhoods>(result);

            return neighborhoods;
        }

        public async Task<Neighborhoods> GetAllNeighborhoods(int skip)
        {
            var response = await _httpClient.GetAsync($"neighborhoods?limit=100&skip={skip}");
            if (!response.IsSuccessStatusCode)
            {
                //todo : throw exception
            }
            var result = await response.Content.ReadAsStringAsync();
            var neighborhoods = JsonConvert.DeserializeObject<Neighborhoods>(result);

            return neighborhoods;
        }
    }
}
