using System;
using System.Collections.Generic;
using System.Text;

namespace realEstate.Common.Enums
{
    public enum AdvertType
    {
        Rent=1,
        Sale=2
    }
    
    public static class AdvertTypeList
    {
        public static Dictionary<int,string> AdvertTypeDictionary = new Dictionary<int, string>()
        {
            {(int)AdvertType.Sale,"satilik"},
            {(int)AdvertType.Rent,"kiralik"}
        };
    }
}
