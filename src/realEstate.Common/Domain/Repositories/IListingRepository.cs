using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using realEstate.Common.Domain.Model;
using realEstate.Common.Enums;

namespace realEstate.Common.Domain.Repositories
{
    public interface IListingRepository
    {
        Task UpsertRecord(Listing record);
        Task DeleteBulkAsync(List<ObjectId> ids);
        // todo : query icin bunu kullanalim. alttakini silelim
        IMongoQueryable<Listing> GetByFilter(Expression<Func<Listing, bool>> predicate);

        Task InsertListing(Listing listing);
        Task<List<Listing>> GetListingsByFilter(string townId, string cityId, int owner);
    }

    public class ListingRepository : IListingRepository
    {
        private readonly IMongoDatabase _database;

        public ListingRepository(IMongoDatabase database)
        {
            _database = database;
        }


        public async Task UpsertRecord(Listing record)
        {
            await Collection.ReplaceOneAsync(
                recordInDb => recordInDb.AdvertId.Equals(record.AdvertId),
                record,
                new ReplaceOptions { IsUpsert = true }
            );
        }

        public async Task InsertListing(Listing listing)
        {
            await Collection.InsertOneAsync(listing);
        }

        public async Task DeleteBulkAsync(List<ObjectId> ids)
        {
            var idsFilter = Builders<Listing>.Filter.In(d => d.Id, ids);
            await Collection.DeleteManyAsync(idsFilter);
        }

        public async Task<List<Listing>> GetListingsByFilter(string townId, string cityId, int owner)
        {
            var listings = await Collection
                .FindAsync(x =>
                                x.Town.Id == townId &&
                                x.City.Id == cityId &&
                                x.OwnerSite == owner);
            return await listings.ToListAsync();
        }

        public IMongoQueryable<Listing> GetByFilter(Expression<Func<Listing, bool>> predicate)
        {
            return Collection.AsQueryable().Where(predicate);
        }

        private IMongoCollection<Listing> Collection
            => _database.GetCollection<Listing>("Listings");
    }
}
