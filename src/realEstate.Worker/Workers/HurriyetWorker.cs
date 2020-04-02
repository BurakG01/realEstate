using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using realEstate.Common.Domain.Model;
using realEstate.Common.Domain.Repositories;
using realEstate.Common.Enums;
using realEstate.Common.ExternalServices;
using realEstate.Common.InternalServices;
using realEstate.Common.Mapper;
using realEstate.Common.ParsingModel.Hurriyet;

namespace realEstate.Worker.Workers
{
    public class HurriyetWorker : BackgroundService
    {
        private readonly ILogger<HurriyetWorker> _logger;
        private readonly IHurriyetService _hurriyetService;
        private readonly IServiceProvider _services;
        private readonly IInternalLocationService _internalLocationService;
        public HurriyetWorker(ILogger<HurriyetWorker> logger,
            IHurriyetService hurriyetService,
            IServiceProvider services,
            IInternalLocationService internalLocationService)
        {
            _logger = logger;
            _hurriyetService = hurriyetService;
            _services = services;
            _internalLocationService = internalLocationService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {

                    var listingRepository = scope.ServiceProvider.GetRequiredService<IListingRepository>();

                    var cities = await _internalLocationService.GetCities();
                    foreach (var city in cities)
                    {
                        foreach (var town in city.Towns)
                        {
                            var townName = GetEnCharactersInString(town.Name.ToLower());

                            var response = await _hurriyetService.
                                GetAdvertsList($"{townName}");

                            var listings =
                                await listingRepository.GetListingsByFilter(town.Id, city.Id, (int)Owners.HurriyetEmlak);
                            var notExistListings =
                                listings.Where(x => response.All(y => GetAdvertId(y.Url.ToString()) != x.AdvertId))
                                    .Select(x => x.Id).ToList();
                            if (notExistListings.Any())
                            {
                                await listingRepository.DeleteBulkAsync(notExistListings);
                            }

                            foreach (var item in response)
                            {
                                var itemUrl = item.Url.ToString();
                                var rentListing = new Listing
                                {
                                    City = new LocationModel { Name = city.Name, Id = city.Id },
                                    Town = new LocationModel { Name = town.Name, Id = town.Id },
                                    Name = item.Name,
                                    Url = itemUrl,
                                    OwnerSite = (int)Owners.HurriyetEmlak,
                                    AdvertId = GetAdvertId(itemUrl)

                                };
                                var detail = await _hurriyetService.GetAdvertDetail(itemUrl);

                                var district = GetRelatedDistrict(town, detail.Offers.ItemOfferedDetail.Address.StreetAddress);
                                var neighborhood = district.Neighborhoods.First();
                                rentListing.Images = detail.Offers.Image.Select(x => x.ContentUrl.ToString()).ToList();
                                rentListing.District = new LocationModel() { Name = district.Name, Id = district.Id };
                                rentListing.Neighborhood = new LocationModel() { Name = neighborhood.Name, Id = neighborhood.Id };
                                rentListing.Price = new PriceModel() { Price = detail.Offers.Price, Currency = detail.Offers.PriceCurrency };
                                rentListing.ShortDescription = detail.Offers.Description;
                                rentListing.FullDescription = detail.FullDescription;
                                rentListing.FullDescriptionInHtml = detail.FullDescriptionInHtml;
                                rentListing.AdvertiseOwner = GetAdvertiseOwner(detail.Offers.Seller);
                                rentListing.AdvertiseOwnerName = detail.Offers.Seller.Name;
                                rentListing.AdvertiseOwnerPhone = detail.Offers.Seller.Telephone;
                                rentListing.RoomNumber =
                                    detail.AdvertFeatures.FirstOrDefault(x => x.Name == "Oda + Salon Sayısı")?.Value;
                                rentListing.AdvertStatus =
                                    detail.AdvertFeatures.FirstOrDefault(x => x.Name == "İlan Durumu")?.Value;
                                rentListing.SquareMeter =
                                    detail.AdvertFeatures.FirstOrDefault(x => x.Name == "Brüt / Net M2")?.Value;
                                rentListing.BuildingAge =
                                    detail.AdvertFeatures.FirstOrDefault(x => x.Name == "Bina Yaşı")?.Value;
                                rentListing.FloorLocation =
                                    detail.AdvertFeatures.FirstOrDefault(x => x.Name == "Bulunduğu Kat")?.Value;
                                rentListing.NumberOfFloor =
                                    detail.AdvertFeatures.FirstOrDefault(x => x.Name == "Kat Sayısı")?.Value;
                                rentListing.FurnitureStatus =
                                    detail.AdvertFeatures.FirstOrDefault(x => x.Name == "Eşya Durumu")?.Value;
                                await listingRepository.UpsertRecord(rentListing);
                            }

                        }
                    }

                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(30 * 60 * 1000, stoppingToken);
            }
        }
        private string GetEnCharactersInString(string text)
        {
            StringBuilder sb = new StringBuilder(text);

            sb.Replace("ı", "i");
            sb.Replace("ü", "u");
            sb.Replace("ç", "c");
            sb.Replace("ö", "o");
            sb.Replace("ü", "u");
            sb.Replace("ğ", "g");
            sb.Replace("ş", "s");

            return sb.ToString();
        }
        private string GetAdvertId(string itemUrl)
        {
            int pos = itemUrl.LastIndexOf("/", StringComparison.Ordinal) + 1;
            return itemUrl.Substring(pos, itemUrl.Length - pos);
        }
        private string GetAdvertiseOwner(Seller seller)
        {
            string advertiseOwner;
            if (seller.Name.IndexOf("bank", StringComparison.CurrentCultureIgnoreCase) > 0)
            {
                advertiseOwner = "Bank";
            }
            else if (seller.Name.IndexOf("emlak", StringComparison.CurrentCultureIgnoreCase) > 0)
            {
                advertiseOwner = "RealEstateAgent";
            }
            else
            {
                advertiseOwner = "Individual";
            }

            return advertiseOwner;
        }
        private District GetRelatedDistrict(Town town, string streetName)
        {
            var returnDistrict = new District();

            foreach (var district in town.Districts)
            {
                foreach (var item in district.Neighborhoods)
                {
                    var substring = item.Name.Substring(0, item.Name.IndexOf("mah", StringComparison.Ordinal)).TrimEnd();
                    if (String.Equals(substring, streetName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        returnDistrict.Name = district.Name;
                        returnDistrict.Id = district.Id;
                        returnDistrict.Neighborhoods = new List<Neighborhood>() { item };
                        break;

                    }
                }

            }
            return returnDistrict;
        }
    }
}
