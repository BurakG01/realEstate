using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace realEstate.Common.Domain.Model
{
    public partial class Location
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        public List<City> Cities { get; set; }
    }
    public class City
    {
        public string Name { get; set; }
        public List<Town> Towns { get; set; }
    }
    public class Town
    {
        public string Name { get; set; }
    
    }
  
}
