using System;
using System.Collections.Generic;
using System.Text;
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
        [JsonProperty("_id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Town { get; set; }
        public string City { get; set; }

    }
}
