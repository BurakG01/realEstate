using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using realEstate.Common.Domain.Model;
using realEstate.Common.ParsingModel;

namespace realEstate.Common.Domain.Repositories
{
    public interface INeighborhoodRepository
    {

        Task AddAsync(List<NeighborhoodData> neighborhoods);
        IMongoQueryable<NeighborhoodData> GetByFilter(Expression<Func<NeighborhoodData, bool>> predicate);
    }

    public class NeighborhoodRepository : INeighborhoodRepository
    {
        private readonly IMongoDatabase _database;

        public NeighborhoodRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task AddAsync(List<NeighborhoodData> neighborhoods)
        {
            await Collection.InsertManyAsync(neighborhoods);
        }

        public IMongoQueryable<NeighborhoodData> GetByFilter(Expression<Func<NeighborhoodData, bool>> predicate)
        {
            return Collection.AsQueryable().Where(predicate);
        }

        private IMongoCollection<NeighborhoodData> Collection
            => _database.GetCollection<NeighborhoodData>("Neighborhoods");
    }
}
