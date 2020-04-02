using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace realEstate.Common.ParsingModel.EmlakJet
{
    public class EjListingBase
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("title")]
        public Description Title { get; set; }
    }

    public partial class Category
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug", NullValueHandling = NullValueHandling.Ignore)]
        public string Slug { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
        public Category City { get; set; }
    }

    public partial class Description
    {
        [JsonProperty("tr")]
        public string Tr { get; set; }
    }

    public partial class Properties
    {
        [JsonProperty("CITY", NullValueHandling = NullValueHandling.Ignore)]
        public Category City { get; set; }

        [JsonProperty("TOWN")]
        public Category Town { get; set; }

        [JsonProperty("PRICE", NullValueHandling = NullValueHandling.Ignore)]
        public Price Price { get; set; }

        [JsonProperty("DISTRICT", NullValueHandling = NullValueHandling.Ignore)]
        public Category District { get; set; }
    }

    public partial class Price
    {
        [JsonProperty("value")]
        public long Value { get; set; }
    }

    public partial class User
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long Id { get; set; }

        [JsonProperty("account_id", NullValueHandling = NullValueHandling.Ignore)]
        public long AccountId { get; set; }

        [JsonProperty("office_id", NullValueHandling = NullValueHandling.Ignore)]
        public long OfficeId { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("category", NullValueHandling = NullValueHandling.Ignore)]
        public long Category { get; set; }

        [JsonProperty("phone", NullValueHandling = NullValueHandling.Ignore)]
        public List<UserPhone> Phone { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("account_name", NullValueHandling = NullValueHandling.Ignore)]
        public string AccountName { get; set; }

        [JsonProperty("logo", NullValueHandling = NullValueHandling.Ignore)]
        public object Logo { get; set; }

        [JsonProperty("logo_full", NullValueHandling = NullValueHandling.Ignore)]
        public object LogoFull { get; set; }

        [JsonProperty("account_product_category", NullValueHandling = NullValueHandling.Ignore)]
        public string AccountProductCategory { get; set; }
    }

    public partial class UserPhone
    {
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public string Source { get; set; }

        [JsonProperty("phones", NullValueHandling = NullValueHandling.Ignore)]
        public List<PhonePhone> Phones { get; set; }
    }

    public partial class PhonePhone
    {
        [JsonProperty("did", NullValueHandling = NullValueHandling.Ignore)]
        public string Did { get; set; }

        [JsonProperty("phone_type", NullValueHandling = NullValueHandling.Ignore)]
        public string PhoneType { get; set; }
    }
}
