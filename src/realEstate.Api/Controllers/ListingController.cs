using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using realEstate.Api.FilterModel;
using realEstate.Common.Domain.Model;
using realEstate.Common.Domain.Repositories;
using realEstate.Common.Enums;
using realEstate.Common.Mapper;
using realEstate.Common.Representation;

namespace realEstate.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingController : ControllerBase
    {
        private readonly IListingRepository _listingRepository;
        private readonly IListingRepresentationMapper _listingMapper;

        public ListingController(IListingRepository listingRepository, IListingRepresentationMapper listingMapper)
        {
            _listingRepository = listingRepository;
            _listingMapper = listingMapper;
        }

        [HttpGet("get-top")]
        public async Task<List<dynamic>> GetTopTenListing()
        {
            var listings = await _listingRepository
                 .GetByFilter(x => !string.IsNullOrEmpty(x.AdvertId))
                 .Take(10)
                 .ToListAsync();
            var listingRepresentation = listings.Select(x => _listingMapper.MapListing(x))
                .GroupBy(x => x.ReSku).Select(x =>
                    new
                    {
                        key = x.Key,
                        owners = x.Select(y => new {ownerName= y.OwnerSite ,Url=y.Url}).ToList(),
                        Listing = x.First()
                    }

                ).ToList<dynamic>();
                

            return listingRepresentation;

        }

        [HttpPost("filterable")]
        public async Task<List<ListingRepresentation>> GetFilteredListing([FromBody] ListingFilter filter)
        {
            var listings = _listingRepository.GetByFilter(x => !string.IsNullOrEmpty(x.AdvertId));

            if (!string.IsNullOrEmpty(filter.City))
            {
                listings = listings.Where(x => x.City.Name == filter.City);
            }
            if (!string.IsNullOrEmpty(filter.Town))
            {
                listings = listings.Where(x => x.Town.Name == filter.Town);
            }
            if (!string.IsNullOrEmpty(filter.Street))
            {
                listings = listings.Where(x => x.Street.Name == filter.Street);
            }

            var filteredListing = await listings.ToListAsync();

            return filteredListing.Select(x => _listingMapper.MapListing(x)).ToList();
        }

    }
}