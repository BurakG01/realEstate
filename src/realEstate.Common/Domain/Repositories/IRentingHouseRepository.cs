﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using realEstate.Common.Domain.Model;

namespace realEstate.Common.Domain.Repositories
{
    public interface IRentingHouseRepository
    {
        Task UpsertRecord(RentListing record);
    }

    public class RentingHouseRepository : IRentingHouseRepository
    {
        private readonly IMongoDatabase _database;

        public RentingHouseRepository(IMongoDatabase database)
        {
            _database = database;
        }


        public async Task UpsertRecord(RentListing record)
        {
            await Collection.ReplaceOneAsync(
                recordInDb => recordInDb.AdvertId.Equals(record.AdvertId),
                record,
                new ReplaceOptions { IsUpsert = true }
            );
        }

        private IMongoCollection<RentListing> Collection
            => _database.GetCollection<RentListing>("RentingListings");
    }
}
