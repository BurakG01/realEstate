using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace realEstate.Common.ParsingModel
{
    public class Towns
    {
        [JsonProperty("Data")]
        public List<Town> TownList { get; set; }

        public bool Status { get; set; }
    }

    public class Town
    {

        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }
    }
}
