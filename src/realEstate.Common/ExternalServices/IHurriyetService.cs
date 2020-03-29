using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
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
            //var response = await _httpClient.GetAsync(url);
            var response = await _httpClient.GetAsync("https://www.hurriyetemlak.com/adana-cukurova-guzelyali-satilik/daire/3334-13019");
            if (!response.IsSuccessStatusCode)
            {
                //todo : throw exception
            }

            var result = await response.Content.ReadAsStringAsync();
            var document = new HtmlDocument();

            document.LoadHtml(result);

            var advertDetail = document.DocumentNode.SelectNodes("//div[@class='det-title-bottom']/following::ul[1]/li");
            var advertDetailDict = new Dictionary<string, string>();
            foreach (var advert in advertDetail)
            {
                var pair = advert.Descendants("span").ToList();
                advertDetailDict.Add(pair[0].InnerText, pair[1].InnerText);

            }

            var script = document.DocumentNode.Descendants()
              .FirstOrDefault(n => n.Name == "script" && n.OuterHtml.Contains("application/ld+json"))
              ?.InnerHtml;
            var description =
                document.DocumentNode.SelectNodes("//div[@class='mt20 ql-editor description-content']");

            JToken token = JObject.Parse(script);
            var itemOffered = token.SelectToken("mainEntity").ToString();
            var itemDetail = JsonConvert.DeserializeObject<ItemDetail>(itemOffered);
            if (description != null)
            {
                itemDetail.FullDescription = description.FirstOrDefault().InnerText;
                itemDetail.FullDescriptionInHtml = description.FirstOrDefault().InnerHtml;
            }

            return itemDetail;

        }

        public async Task<List<ItemOffered>> GetAdvertsList(string url)
        {
            var finalList = new List<ItemOffered>();
            var page = 1;
            while (true)
            {
                Thread.Sleep(5 * 1000);
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
