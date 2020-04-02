using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using realEstate.Common.Domain.Model;

namespace realEstate.Common.Domain.Repositories
{
    public interface ILocationRepository
    {
        Task AddAsync(City location);
        Task<List<City>> GetAsync();
    }
    public class LocationRepository : ILocationRepository
    {
        private readonly IMongoDatabase _database;

        public LocationRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<List<City>> GetAsync()
        {
            return await Collection.Find(_ => true).ToListAsync();

        }

        public async Task AddAsync(City city)
        {
            await Collection.InsertOneAsync(city);
        }

        private IMongoCollection<City> Collection
            => _database.GetCollection<City>("Cities");
    }
}
