using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace realEstate.Api.FilterModel
{
    public class ListingFilter
    {
        public string City { get; set; }
        public string Town { get; set; }
        public string Street { get; set; }
        public List<string> RoomNumber { get; set; }
        public string AdvertOwnerType { get; set; } // RealEstateAgent , Personal , Other or EmptyString = For All
        public string FurnitureType { get; set; } // Eşyalı, Boş or EmptyString=For All
        public string AdvertStatus { get; set; } // Kiralik - satilik or emptystring
        public int PageNumber { get; set; }
    }
}
