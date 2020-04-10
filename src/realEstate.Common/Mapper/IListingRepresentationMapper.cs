using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using realEstate.Common.Domain.Model;
using realEstate.Common.Enums;
using realEstate.Common.Representation;

namespace realEstate.Common.Mapper
{
    public interface IListingRepresentationMapper
    {
        ListingRepresentation MapListing(Listing listing);
    }

    public class ListingRepresentationMapper : IListingRepresentationMapper
    {
        private readonly IMapper _mapper;

        public ListingRepresentationMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Listing, ListingRepresentation>()
                    .ForMember(dst => dst.Id,
                        opt =>
                            opt.MapFrom(x => x.Id.ToString()))
                    .ForMember(dst => dst.City,
                        opt =>
                            opt.MapFrom(x => x.City.Name))
                    .ForMember(dst => dst.Town,
                        opt =>
                            opt.MapFrom(x => x.Town.Name))
                    .ForMember(dst => dst.Street,
                        opt =>
                            opt.MapFrom(x => x.Street.Name))
                    .ForMember(dst => dst.CoverImage,
                        opt =>
                            opt.MapFrom(x => x.Images.FirstOrDefault()))
                    .ForMember(dst => dst.OwnerSite,
                        opt =>
                            opt.MapFrom(x => ((Owners)x.OwnerSite).ToString()))

                    .ForMember(dst => dst.Price,
                        opt =>
                            opt.MapFrom(x => $"{x.Price.Price} {x.Price.Currency}"));
            });
            _mapper = config.CreateMapper();
        }

        public ListingRepresentation MapListing(Listing listing)
        {
            return _mapper.Map<Listing, ListingRepresentation>(listing);
        }
    }
}
