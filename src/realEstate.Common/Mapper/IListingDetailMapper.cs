using System;
using System.Collections.Generic;
using System.Text;
using realEstate.Common.Domain.Model;
using realEstate.Common.ParsingModel;
using realEstate.Common.ParsingModel.Hurriyet;

namespace realEstate.Common.Mapper
{
    public interface IListingDetailMapper
    {
        Listing MapListing(ItemDetail itemDetail, ItemOffered itemOffered);
    }
}
