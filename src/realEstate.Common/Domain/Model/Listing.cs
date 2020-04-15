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
        public string ReSku { get; set; } // burada bizim verecegimiz unique bir id
        public string AdvertStatus { get; set; }// kiralik - satilik -gunluk kiralik
        public int OwnerSite { get; set; }// ilani hangi siteden aldik
        public string AdvertOwnerType { get; set; }// emlakci - sahibinden-bankadan
        public string AdvertOwnerName { get; set; }// ilan sahibinin ismi
        public string AdvertOwnerPhone { get; set; }// ilan sahibinin telefonu
        public string RoomNumber { get; set; } // oda sayisi
        public string FloorLocation { get; set; } // bulundugu kat
        public string NumberOfFloor { get; set; } // Kat sayisi
        public string FurnitureStatus { get; set; } // Esya durumu
        public string SquareMeter { get; set; } // evin metrekaresi
        public string BuildingAge { get; set; } // bina yasi
        public string HeatingType { get; set; }// isinma tipi
        public string Name { get; set; }
        public string Url { get; set; } // ilan detay linki 
        public List<string> Images { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string FullDescriptionInHtml { get; set; }
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

    public partial class AdvertFeatureModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
