using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace realEstate.Common.ParsingModel
{
    public class Districts
    {
        public bool Status { get; set; }
        [JsonProperty("Data")]
        public List<DistrictData> DistrictList { get; set; }
    }

    public class DistrictData
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Town{ get; set; }
        public string City{ get; set; }


    }
}
