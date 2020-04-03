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
using realEstate.Common.Domain.Model;
using realEstate.Common.ParsingModel.Hurriyet;

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
            Thread.Sleep(1 * 1000);
            if (!response.IsSuccessStatusCode)
            {
                //todo : throw exception
            }

            var result = await response.Content.ReadAsStringAsync();
            var document = new HtmlDocument();

            document.LoadHtml(result);

            var advertDetail = document.DocumentNode.SelectNodes("//div[@class='det-title-bottom']/following::ul[1]/li");

            var advertFeaturesList = advertDetail.Select(x => new AdvertFeatureModel
            {
                Name = x.Descendants("span").First().InnerText,
                Value = x.Descendants("span").Skip(1).Select(y => y.InnerText).Aggregate((i, j) => i + "," + j)

            }).ToList();

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
            itemDetail.AdvertFeatures = advertFeaturesList;

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
                var withoutProjectItemOfferedList = itemOfferedList.Where(x => !x.Url.AbsolutePath.Contains("/projeler/")).ToList();
                finalList.AddRange(withoutProjectItemOfferedList);
                if (itemOfferedList.Count < 24)
                {
                    break;
                }
                Thread.Sleep(1 * 1000);

                page++;

            }
            return finalList;
        }
    }
}
