using realEstate.Common.ParsingModel;
using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace realEstate.Common.Domain.Model
{
    public class Listing
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        public string AdvertId { get; set; } // cektigimiz yerden aldigimiz unique id
        public string ReSku { get; set; } // bizim koyacagimiz unique bir alan
        public int AdvertType { get; set; }// kiralik - satilik 
        public int OwnerSite { get; set; }// ilani hangi siteden aldik
        public int AdvertiseOwner { get; set; }// emlakci - sahibinden
        public string Name { get; set; }
        public string RoomNumber { get; set; }
        public string HeatingType { get; set; }
        public string BuildingAge { get; set; }
        public string HousingType { get; set; }
        public string FloorLocation { get; set; }
        public string NumberOfFloor { get; set; }
        public string FuelType { get; set; }
        public string Size { get; set; }
        public string UsingStatus { get; set; }
        public string BuildingType { get; set; }
        public string FurnishedStatus { get; set; }
        public string Url { get; set; } // ilan detay linki 
        public List<string> Images { get; set; }
        public Description Description { get; set; }
        public PriceModel Price { get; set; }
        public LocationModel City { get; set; }
        public LocationModel Town { get; set; }
        public LocationModel Street { get; set; }
    }

    public partial class LocationModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public partial class PriceModel
    {
        public long Price { get; set; }
        public string Currency { get; set; }

    }
    public partial class Description
    {
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string FullDescriptionInHtml { get; set; }
    }
}
