using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace realEstate.Common.ParsingModel
{
    public  class Cities
    {
        public bool Status { get; set; }
       
        public List<Data> Data { get; set; }
    }

    public  class Data
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
