using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using realEstate.Common.Domain.Model;
using realEstate.Common.Domain.Repositories;
using realEstate.Common.Enums;
using realEstate.Common.ExternalServices;
using realEstate.Common.InternalServices;

namespace realEstate.Worker.Workers
{
    public class EmlakJetWorker : BackgroundService
    {
        private readonly ILogger<EmlakJetWorker> _logger;
        private readonly IEmlakJetService _emlakJetService;
        private readonly IServiceProvider _services;
        private readonly IInternalLocationService _internalLocationService;

        public EmlakJetWorker(ILogger<EmlakJetWorker> logger, IEmlakJetService emlakJetService, IServiceProvider services, IInternalLocationService internalLocationService)
        {
            _logger = logger;
            _emlakJetService = emlakJetService;
            _services = services;
            _internalLocationService = internalLocationService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var advertStatuses = new Dictionary<string, string>()
                {
                    {"kiralik","Kiralık" },
                    {"satilik","Satılık" }
                };
                using (var scope = _services.CreateScope())
                {
                    var listingRepository = scope.ServiceProvider.GetRequiredService<IListingRepository>();
                    var cities = await _internalLocationService.GetCities();
                    foreach (var city in cities)
                    {
                        foreach (var town in city.Towns)
                        {
                            var cityName = GetEnCharactersInString(city.Name.ToLower());
                            var townName = GetEnCharactersInString(town.Name.ToLower());
                            foreach (var advertStatus in advertStatuses)
                            {
                                var advertList =
                                    await _emlakJetService.GetAdvertList($"{advertStatus.Key}-konut/{cityName}-{townName}");
                                var listings =
                                    await listingRepository.GetListingsByFilter(town.Id, city.Id, (int)Owners.EmlakJet);

                                var notExistListings =
                                    listings.Where(x => advertList.All(y => y.Id.ToString() != x.AdvertId))
                                        .Select(x => x.Id).ToList();
                                if (notExistListings.Any())
                                {
                                    await listingRepository.DeleteBulkAsync(notExistListings);
                                }

                                foreach (var ejListing in advertList)
                                {
                                    var url = $"https://www.emlakjet.com{ejListing.Url}";
                                    var reSku = $"RE{(int)Owners.EmlakJet}{ejListing.Id}";
                                    if (listings.FirstOrDefault(x => x.AdvertId.Equals(ejListing.Id.ToString()))
                                        .ReSku == reSku)
                                    {
                                        var listing = new Listing()
                                        {
                                            City = new LocationModel { Name = city.Name, Id = city.Id },
                                            Town = new LocationModel { Name = town.Name, Id = town.Id },
                                            Name = ejListing.Title.Tr,
                                            Url = url,
                                            OwnerSite = (int)Owners.EmlakJet,
                                            AdvertId = ejListing.Id.ToString(),
                                            AdvertStatus = advertStatus.Value,
                                            ReSku = reSku
                                        };

                                        var listingDetail = await _emlakJetService.GetAdvertDetail(url);
                                        listing.Price = new PriceModel() { Price = listingDetail.PriceOrder, Currency = listingDetail.Currency };
                                        listing.Images = listingDetail.ImagesFull.Select(x => $"https://imaj.emlakjet.com{x}").ToList();
                                        listing.Street = new LocationModel() { Name = listingDetail.Properties.Town.Name };
                                        listing.FullDescriptionInHtml = listingDetail.DescriptionMasked.Tr;
                                        listing.FullDescription = GetClearText(listingDetail.DescriptionMasked.Tr);
                                        listing.AdvertiseOwner =
                                            listingDetail.OrderedProperties.FirstOrDefault(x => x.Title == "Kimden")?.Value == "Emlak Ofisinden" ?
                                                "RealEstateAgent" : "Individual";
                                        listing.AdvertiseOwnerName = listingDetail.User.FullName;
                                        listing.AdvertiseOwnerPhone = listingDetail.PhoneNumber;
                                        listing.RoomNumber =
                                            listingDetail.OrderedProperties.FirstOrDefault(x => x.Title == "Oda Sayısı")?.Value;

                                        listing.SquareMeter =
                                          $"{listingDetail.OrderedProperties.FirstOrDefault(x => x.Title == "Brüt m2")?.Value} / " +
                                          $"{listingDetail.OrderedProperties.FirstOrDefault(x => x.Title == "Net m2")?.Value}";

                                        listing.BuildingAge =
                                            listingDetail.OrderedProperties.FirstOrDefault(x => x.Title == "Bina Yaşı")?.Value;
                                        listing.FloorLocation =
                                            listingDetail.OrderedProperties.FirstOrDefault(x => x.Title == "Bulunduğu Kat")?.Value;
                                        listing.NumberOfFloor =
                                            listingDetail.OrderedProperties.FirstOrDefault(x => x.Title == "Kat Sayısı")?.Value;
                                        listing.FurnitureStatus =
                                            listingDetail.OrderedProperties.FirstOrDefault(x => x.Title == "Eşya Durumu")?.Value;
                                        listing.HeatingType =
                                            listingDetail.OrderedProperties.FirstOrDefault(x => x.Title == "Isıtma")?.Value;

                                        await listingRepository.UpsertRecord(listing);
                                    }

                                }

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

        private string GetClearText(string htmlContent)
        {
            var step1 = Regex.Replace(htmlContent, @"<[^>]+>|&nbsp;", "").Trim();
            var step2 = Regex.Replace(step1, @"\s{2,}", " ");
            StringBuilder sb = new StringBuilder(step2);

            sb.Replace("&Ccedil;", "Ç");
            sb.Replace("&ccedil;", "ç");
            sb.Replace("&#286;", "Ğ");
            sb.Replace("&#287;", "ğ");
            sb.Replace("&#304;", "İ");
            sb.Replace("&#305;", "ı");
            sb.Replace("&Ouml;", "Ö");
            sb.Replace("&#350;", "Ş");
            sb.Replace("&#351;", "ş");
            sb.Replace("&Uuml;", "Ü");
            sb.Replace("&uuml;", "ü");
            sb.Replace("&sup2;", "²");

            return sb.ToString();
        }
    }
}
