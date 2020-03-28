using realEstate.Common.ParsingModel;
using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace realEstate.Common.Domain.Model
{
   public class RentListing
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string ReSku { get; set; }
        public string Owner { get; set; }
        public Url Url{ get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription{ get; set; }
        public List<string> Image { get; set; }
        public string AdvertId { get; set; }
        public long Price { get; set; }
        public List<LocationModel> Locations { get; set; } 
    }

   public partial class LocationModel
   {
       public string Type { get; set; }
        public string Name { get; set; }
   }

   public partial class Url
   {
       public string Owner { get; set; }
       public string Link{ get; set; }
   }
}
