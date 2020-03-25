using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using realEstate.Common.ParsingModel;

namespace realEstate.Common.ExternalServices
{
    public interface IHurriyetService
    {
        Task<List<ItemOffered>> GetAdvertsList(string url);
        Task<ItemDetail> GetAdvertDetail(string url);
    }

    public class HurriyetService : IHurriyetService
    {
        private readonly HttpClient _httpClient;

        public HurriyetService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ItemDetail> GetAdvertDetail(string url)
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
              .FirstOrDefault(n => n.Name == "script" && n.OuterHtml.Contains("application/ld+json"))
              ?.InnerHtml;
            JToken token = JObject.Parse(script);
            var itemOffered = token.SelectToken("mainEntity").ToString();
            var itemDetail = JsonConvert.DeserializeObject<ItemDetail>(itemOffered);
            return itemDetail;
        }

        public async Task<List<ItemOffered>> GetAdvertsList(string url)
        {
            var finalList = new List<ItemOffered>();
            var page = 1;
            while (true)
            {

                var response = await _httpClient.GetAsync($"{url}?page={page}");

                if (!response.IsSuccessStatusCode)
                {
                    //todo : throw exception
                }

                var result = await response.Content.ReadAsStringAsync();
                var document = new HtmlDocument();
                document.LoadHtml(result);

                var script = document.DocumentNode.Descendants()
                    .FirstOrDefault(n => n.Name == "script" && n.OuterHtml.Contains("application/ld+json"))
                    ?.InnerHtml;


                JToken token = JObject.Parse(script);

                var itemOffered = token.SelectToken("mainEntity.offers.itemOffered").ToString();
                var itemOfferedList = JsonConvert.DeserializeObject<List<ItemOffered>>(itemOffered);
                finalList.AddRange(itemOfferedList);
                if (itemOfferedList.Count < 24)
                {
                    break;
                }

                page++;

            }
            return finalList;
        }
    }
}
