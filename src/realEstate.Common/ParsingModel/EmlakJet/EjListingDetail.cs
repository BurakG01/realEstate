using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace realEstate.Common.ParsingModel.EmlakJet
{
    public class EjListingDetail : EjListingBase
    {
        [JsonProperty("orderedProperties")]
        public List<OrderedProperty> OrderedProperties { get; set; }
        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("price_order")]
        public string PriceOrder { get; set; }

        [JsonProperty("cover_photo")]
        public string CoverPhoto { get; set; }

        [JsonProperty("has_photo")]
        public bool HasPhoto { get; set; }

        [JsonProperty("special_promo_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SpecialPromoId { get; set; }


        [JsonProperty("description")]
        public Description Description { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("revised_at")]
        public DateTimeOffset RevisedAt { get; set; }

        [JsonProperty("images")]
        public List<string> Images { get; set; }

        [JsonProperty("images_full")]
        public List<string> ImagesFull { get; set; }

        [JsonProperty("cover_photo_full")]
        public string CoverPhotoFull { get; set; }

        [JsonProperty("cover_photo_url")]
        public string CoverPhotoUrl { get; set; }


        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("descriptionMasked")]
        public Description DescriptionMasked { get; set; }
    }

    public partial class OrderedProperty
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
