using realEstate.Common.ParsingModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace realEstate.Common.Domain.Model
{
   public class HurriyetModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Address Address { get; set; }
        public Seller Seller { get; set; }
        public List<string> Image { get; set; }
        public Offers Offers { get; set; }
    }
}
