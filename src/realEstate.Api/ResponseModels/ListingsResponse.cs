using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace realEstate.Api.ResponseModels
{
    public class ListingsResponse
    {
        public int TotalCount { get; set; }
        public  List<dynamic> Listings { get; set; }
    }
}
