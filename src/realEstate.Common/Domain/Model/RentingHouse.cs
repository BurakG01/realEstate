using realEstate.Common.ParsingModel;
using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace realEstate.Common.Domain.Model
{
   public class RentingHouse
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public List<string> FromList { get; set; }
        public List<string> PathList{ get; set; }
        public string Description { get; set; }
        public Address Address { get; set; }
        public Seller Seller { get; set; }
        public List<string> Image { get; set; }
        public Offers Offers { get; set; }
        public string AdvertId { get; set; }
        public List<LocationModel> Locations { get; set; } 
    }

   public partial class LocationModel
   {
       public string Type { get; set; }
        public string Name { get; set; }
   }
}
