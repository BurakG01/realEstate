using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace realEstate.Api.FilterModel
{
    public class ListingFilter
    {
        public string City { get; set; }
        public List<string> Towns { get; set; } // Multiple Choices
        public List<string> Streets { get; set; }// Multiple Choices
        public List<string> RoomNumber { get; set; }
        public string AdvertOwnerType { get; set; } // RealEstateAgent , Personal , Other or EmptyString = For All
        public string FurnitureType { get; set; } // Eşyalı, Boş or EmptyString=For All
        public string AdvertStatus { get; set; } // Kiralik - satilik or empty-string
        public int PageNumber { get; set; }
    }
}
