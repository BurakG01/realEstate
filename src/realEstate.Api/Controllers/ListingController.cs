using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using realEstate.Api.FilterModel;
using realEstate.Api.ResponseModels;
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
                        owners = x.Select(y => new { ownerName = y.OwnerSite, Url = y.Url }).ToList(),
                        Listing = x.First()
                    }

                ).ToList<dynamic>();


            return listingRepresentation;

        }

        [HttpPost("filterable")]
        public async Task<ListingsResponse> GetFilteredListing([FromBody] ListingFilter filter)
        {
            var listings = _listingRepository.GetByFilter(x => !string.IsNullOrEmpty(x.AdvertId));

            if (!string.IsNullOrEmpty(filter.City))
            {
                listings = listings.Where(x => x.City.Name == filter.City);
            }
            if (filter.Towns != null && filter.Towns.Any())
            {
                listings = listings.Where(x => filter.Towns.Contains(x.Town.Name));
            }
            if (filter.Streets != null && filter.Streets.Any())
            {
                listings = listings.Where(x => filter.Streets.Contains(x.Street.Name));
            }

            if (filter.RoomNumber != null && filter.RoomNumber.Any())
            {
                listings = listings.Where(x => filter.RoomNumber.Any(y => x.RoomNumber == y));
            }

            if (!string.IsNullOrEmpty(filter.AdvertStatus))
            {

                listings = listings.Where(x => x.AdvertStatus == filter.AdvertStatus);

            }
            if (!string.IsNullOrEmpty(filter.FurnitureType))
            {
                listings = listings.Where(x => x.FurnitureStatus == filter.FurnitureType);
            }
            if (!string.IsNullOrEmpty(filter.AdvertOwnerType))
            {
                listings = listings.Where(x => x.AdvertOwnerType == filter.AdvertOwnerType);
            }

            var totalPage = Math.Ceiling((double)listings.Count() / 20);

            listings = listings.Skip(filter.PageNumber * 20).Take(20);

            var filteredListing = await listings.ToListAsync();

            var listingRepresentation = filteredListing.Select(x => _listingMapper.MapListing(x))
                .GroupBy(x => x.ReSku).Select(x =>
                    new
                    {
                        key = x.Key,
                        owners = x.Select(y => new { ownerName = y.OwnerSite, Url = y.Url }).ToList(),
                        Listing = x.First()
                    }

                ).ToList<dynamic>();
            return new ListingsResponse
            {
                TotalPage =(int)totalPage,
                Listings = listingRepresentation
            };
        }

    }
}