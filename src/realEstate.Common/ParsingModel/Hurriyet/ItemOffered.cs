using System;
using Newtonsoft.Json;

namespace realEstate.Common.ParsingModel.Hurriyet
{
    public class ItemOffered
    {
        [JsonProperty("@type")]
        public Uri[] Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("additionalType")]
        public Uri AdditionalType { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }

        [JsonProperty("offers")]
        public Offers Offers { get; set; }
    }

    public class Address
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

    public class Offers
    {
        [JsonProperty("BusinessFunction")]
        public Uri BusinessFunction { get; set; }

        [JsonProperty("availability")]
        public Uri Availability { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("priceCurrency")]
        public string PriceCurrency { get; set; }

        [JsonProperty("image")]
        public Uri Image { get; set; }
    }
}
