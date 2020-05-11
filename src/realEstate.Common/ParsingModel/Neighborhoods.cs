using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace realEstate.Common.ParsingModel
{
    public class Neighborhoods
    {
        public bool Status { get; set; }
        [JsonProperty("Data")]
        public List<NeighborhoodData> NeighborhoodList { get; set; }
    }
    public class NeighborhoodData
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        [JsonProperty("_id")]
        public string _Id { get; set; }
        public string Name { get; set; }
        public string Town { get; set; }
        public string District { get; set; }
        public string City { get; set; }
    }
}
