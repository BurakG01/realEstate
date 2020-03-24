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
        Task AddAsync(Location location);
        Task<Location> GetAsync();
    }
    public class LocationRepository:ILocationRepository
    {
        private readonly IMongoDatabase _database;

        public LocationRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<Location> GetAsync()
        {
        var location= await Collection.Find(_ => true).ToListAsync();
        return location.FirstOrDefault();
        }

        public async Task AddAsync(Location location)
        {
            await Collection.InsertOneAsync(location);
        }

        private IMongoCollection<Location> Collection
            => _database.GetCollection<Location>("Locations");
    }
}
