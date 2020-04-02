using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using realEstate.Common.ParsingModel.EmlakJet;
namespace realEstate.Common.ExternalServices
{
    public interface IEmlakJetService
    {
        Task<List<EjListing>> GetAdvertList(string url);
        Task<EjListingDetail> GetAdvertDetail(string url);
    }

    public class EmlakJetService : IEmlakJetService
    {
        private readonly HttpClient _httpClient;

        public EmlakJetService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<EjListing>> GetAdvertList(string url)
        {
            var finalList = new List<EjListing>();
            var page = 1;
            while (true)
            {

                var response = await _httpClient.GetAsync($"{url}/{page}");

                if (!response.IsSuccessStatusCode)
                {
                    //todo : throw exception
                }

                var result = await response.Content.ReadAsStringAsync();
                var document = new HtmlDocument();
                document.LoadHtml(result);

                var script = document.DocumentNode.Descendants()
                    .FirstOrDefault(n => n.Name == "script" && n.Id == "__NEXT_DATA__")
                    ?.InnerHtml;
                if (script != null)
                {

                    JToken token = JObject.Parse(script);

                    var ejListing = token.SelectToken("props.initialProps.pageProps.pageResponse.listingData.listing").ToString();
                    var listings = JsonConvert.DeserializeObject<List<EjListing>>(ejListing);
                    finalList.AddRange(listings);
                    if (listings.Count < 30)
                    {
                        break;
                    }
                    Thread.Sleep(1 * 1000);
                    page++;
                }
                else
                {
                    break;
                }
            }
            return finalList;
        }

        public async Task<EjListingDetail> GetAdvertDetail(string url)
        {
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                //todo : throw exception
            }

            var result = await response.Content.ReadAsStringAsync();
            var document = new HtmlDocument();
            document.LoadHtml(result);

            var script = document.DocumentNode.Descendants()
                .FirstOrDefault(n => n.Name == "script" && n.Id == "__NEXT_DATA__")
                ?.InnerHtml;
            JToken token = JObject.Parse(script);

            var ejListing = token.SelectToken("props.initialProps.pageProps.pageResponse.listing").ToString();
            var listing = JsonConvert.DeserializeObject<EjListingDetail>(ejListing);
            return listing;
        }
    }
}
