using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using realEstate.Common.Domain.Model;
using realEstate.Common.ParsingModel;
using realEstate.Common.ParsingModel.Hurriyet;

namespace realEstate.Common.Mapper
{
    public class ListingDetailMapper: IListingDetailMapper
    {
        private readonly IMapper _mapper;
        public LocationModel City { get; set; }
        public LocationModel Town { get; set; }

        public ListingDetailMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Tuple<ItemDetail, ItemOffered>, Listing>()
                    .ForMember(dst => dst.Id, 
                        opt => opt.Ignore())
                    .ForMember(dst => dst.Description,
                        opt => opt.MapFrom(x=> new Description()
                        {
                            FullDescription = x.Item1.FullDescription,
                            ShortDescription = x.Item1.Offers.Description,
                            FullDescriptionInHtml = x.Item1.FullDescriptionInHtml
                        }))
                    .ForMember(dst => dst.Street,
                        opt => opt.MapFrom(x => new LocationModel()
                        {
                            Name = x.Item1.Offers.ItemOfferedDetail.Address.StreetAddress
                        }))
                    .ForMember(dst => dst.Street,
                        opt => opt.MapFrom(x => new PriceModel()
                        {
                            Price = x.Item1.Offers.Price,
                            Currency = x.Item1.Offers.PriceCurrency

                        }))
                    .ForMember(dst => dst.Name,
                        opt => opt.MapFrom(x=>x.Item2.Name))

                    .ForMember(dst => dst.Url,
                        opt => opt.MapFrom(x => x.Item2.Url.ToString()))
                    .ForMember(dst => dst.Images,
                        opt => 
                            opt.MapFrom(x => x.Item1.Offers.Image.Select(y=>y.ContentUrl.ToString()).ToList()))
                    .ForMember(dst => dst.Street,
                        opt => opt.Ignore())
                    .ForMember(dst => dst.Price,
                        opt => opt.Ignore());
            });
            _mapper = config.CreateMapper();
        }


        public Listing MapListing(ItemDetail itemDetail, ItemOffered itemOffered)
        {
            return _mapper.Map< Tuple<ItemDetail ,ItemOffered>, Listing>(new Tuple<ItemDetail, ItemOffered>(itemDetail,itemOffered));
        }
    }
}
