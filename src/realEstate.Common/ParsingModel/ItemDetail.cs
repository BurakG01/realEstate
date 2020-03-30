using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace realEstate.Common.ParsingModel
{
    public partial class ItemDetail
    {
        [JsonProperty("@type")]
        public string Type { get; set; }
        public string FullDescription { get; set; }
        public string FullDescriptionInHtml { get; set; }
        public  Dictionary<string,string> AdvertFeatures{ get; set; }
        [JsonProperty("offers")]
        public ItemDetailOffers Offers { get; set; }
    }

    public partial class ItemDetailOffers
    {
        [JsonProperty("@type")]
        public Uri Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("price")]
        public long Price { get; set; }

        [JsonProperty("priceCurrency")]
        public string PriceCurrency { get; set; }

        [JsonProperty("BusinessFunction")]
        public Uri BusinessFunction { get; set; }

        [JsonProperty("seller")]
        public Seller Seller { get; set; }

        [JsonProperty("itemOffered")]
        public ItemOfferedDetail ItemOfferedDetail { get; set; }

        [JsonProperty("image")]
        public Image[] Image { get; set; }
    }

    public partial class Image
    {
        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("contentUrl")]
        public Uri ContentUrl { get; set; }

        [JsonProperty("caption")]
        public string Caption { get; set; }
    }

    public partial class ItemOfferedDetail
    {
        [JsonProperty("@type")]
        public Uri[] Type { get; set; }

        [JsonProperty("hasMap")]
        public Uri HasMap { get; set; }

        [JsonProperty("additionalType")]
        public Uri AdditionalType { get; set; }

        [JsonProperty("address")]
        public AddressDetail Address { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("offers")]
        public ItemOfferedOffers Offers { get; set; }
    }

    public partial class AddressDetail
    {
        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("addressLocality")]
        public string AddressLocality { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("streetAddress")]
        public string StreetAddress { get; set; }

        [JsonProperty("addressCountry")]
        public string AddressCountry { get; set; }
    }

    public partial class ItemOfferedOffers
    {
        [JsonProperty("price")]
        public long Price { get; set; }
    }

    public partial class Seller
    {
        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("image")]
        public Uri Image { get; set; }

        [JsonProperty("telephone")]
        public string Telephone { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }

   


}
