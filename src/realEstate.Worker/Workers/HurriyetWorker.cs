using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
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
                            try
                            {
                                var townName = GetEnCharactersInString(town.Name.ToLower());

                                var response = await _hurriyetService.
                                    GetAdvertsList($"{townName}");

                                var listings =
                                    await listingRepository.GetByFilter(x =>
                                            x.City.Name == city.Name &&
                                            x.Town.Name == town.Name &&
                                            x.OwnerSite == (int)Owners.HurriyetEmlak)
                                        .ToListAsync(cancellationToken: stoppingToken);

                                var notExistListings =
                                    listings.Where(x => response.All(y => GetAdvertId(y.Url.ToString()) != x.AdvertId))
                                        .Select(x => x.Id).ToList();
                                if (notExistListings.Any())
                                {
                                    await listingRepository.DeleteBulkAsync(notExistListings);
                                }

                                var newAdverts = response.Where(x => listings
                                        .All(y => y.AdvertId != GetAdvertId(x.Url.ToString())))
                                    .ToList();

                                foreach (var item in newAdverts)
                                {
                                    try
                                    {
                                        var itemUrl = item.Url.ToString();
                                        var advertId = GetAdvertId(itemUrl);
                                        var reSku = $"RE{(int)Owners.HurriyetEmlak}{advertId}";
                                        var listing = new Listing
                                        {
                                            City = new LocationModel { Name = city.Name, Id = city.Id },
                                            Town = new LocationModel { Name = town.Name, Id = town.Id },
                                            Name = item.Name,
                                            Url = itemUrl,
                                            OwnerSite = (int)Owners.HurriyetEmlak,
                                            AdvertId = advertId,
                                            ReSku = reSku

                                        };
                                        var detail = await _hurriyetService.GetAdvertDetail(itemUrl);
                                        listing.Images = detail.Offers.Image.Select(x => x.ContentUrl.ToString()).ToList();
                                        listing.Street = new LocationModel() { Name = detail.Offers.ItemOfferedDetail.Address.StreetAddress };
                                        listing.Price = new PriceModel() { Price = detail.Offers.Price, Currency = detail.Offers.PriceCurrency };
                                        listing.ShortDescription = detail.Offers.Description;
                                        listing.FullDescription = detail.FullDescription;
                                        listing.FullDescriptionInHtml = detail.FullDescriptionInHtml;
                                        listing.AdvertOwnerType = detail.IsPersonal ? "Personal" : GetAdvertiseOwner(detail.Offers.Seller);
                                        listing.AdvertOwnerName = detail.Offers.Seller.Name;
                                        listing.AdvertOwnerPhone = detail.Offers.Seller.Telephone;

                                        var furnitureStatus =
                                            detail.AdvertFeatures.FirstOrDefault(x => x.Name == "Eşya Durumu")?.Value;
                                        var advertStatus = detail.AdvertFeatures.FirstOrDefault(x => x.Name == "İlan Durumu")
                                            ?.Value;
                                        if (!string.IsNullOrEmpty(advertStatus))
                                        {
                                            listing.AdvertStatus = advertStatus == "Günlük Kiralık" ? "Kiralık" : advertStatus;

                                        }

                                        if (!string.IsNullOrEmpty(furnitureStatus))
                                        {
                                            listing.FurnitureStatus =
                                                furnitureStatus == "Eşyalı Değil" ? "Boş" : furnitureStatus;
                                        }

                                        listing.RoomNumber =
                                            detail.AdvertFeatures.FirstOrDefault(x => x.Name == "Oda + Salon Sayısı")?.Value;

                                

                                        listing.SquareMeter =
                                            detail.AdvertFeatures.FirstOrDefault(x => x.Name == "Brüt / Net M2")?.Value;
                                        listing.BuildingAge =
                                            detail.AdvertFeatures.FirstOrDefault(x => x.Name == "Bina Yaşı")?.Value;
                                        listing.FloorLocation =
                                            detail.AdvertFeatures.FirstOrDefault(x => x.Name == "Bulunduğu Kat")?.Value;
                                        listing.NumberOfFloor =
                                            detail.AdvertFeatures.FirstOrDefault(x => x.Name == "Kat Sayısı")?.Value;

                                        listing.HeatingType =
                                            detail.AdvertFeatures.FirstOrDefault(x => x.Name == "Yakıt Tipi")?.Value;

                                        await listingRepository.InsertListing(listing);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    
                                    }

                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                          
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
            string originalCulture = CultureInfo.CurrentCulture.Name;
            string advertiseOwner;
            var otherNames = new List<string>()
            {
                "a.ş",
                "bank",
                "ltd",
                "grup",
                "HOLDİNG",
                "AŞ.",
                "İNŞAAT",
                "Gayrimenkul",
                "YAPI",
                "GROUP",
                "MÜHENDİSLİK",
                "LTD",
                "ŞTİ"
            };
            Thread.CurrentThread.CurrentCulture = new CultureInfo("TR-tr");
            if (seller.Name.IndexOf("emlak", StringComparison.CurrentCultureIgnoreCase) > 0)
            {
                advertiseOwner = "RealEstateAgent";
            }
            else
            {
                advertiseOwner = "Other";
            }
            Thread.CurrentThread.CurrentCulture = new CultureInfo(originalCulture);
            return advertiseOwner;
        }
        //private District GetRelatedDistrict(Town town, string streetName)
        //{
        //    var returnDistrict = new District();

        //    foreach (var district in town.Districts)
        //    {
        //        foreach (var item in district.Neighborhoods)
        //        {
        //            var substring = item.Name.Substring(0, item.Name.IndexOf("mah", StringComparison.Ordinal)).TrimEnd();
        //            if (String.Equals(substring, streetName, StringComparison.CurrentCultureIgnoreCase))
        //            {
        //                returnDistrict.Name = district.Name;
        //                returnDistrict.Id = district.Id;
        //                returnDistrict.Neighborhoods = new List<Neighborhood>() { item };
        //                break;

        //            }
        //        }

        //    }
        //    return returnDistrict;
        //}
    }
}
