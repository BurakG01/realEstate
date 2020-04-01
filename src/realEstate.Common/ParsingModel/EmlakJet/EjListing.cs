using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace realEstate.Common.ParsingModel.EmlakJet
{
   
    
    public partial class EjListing
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("price_order")]
        public long PriceOrder { get; set; }

        [JsonProperty("cover_photo")]
        public string CoverPhoto { get; set; }

        [JsonProperty("has_photo")]
        public bool HasPhoto { get; set; }

        [JsonProperty("map")]
        public Map Map { get; set; }

        [JsonProperty("special_promo_id", NullValueHandling = NullValueHandling.Ignore)]
        public object SpecialPromoId { get; set; }

        [JsonProperty("title")]
        public Description Title { get; set; }

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

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("descriptionMasked")]
        public Description DescriptionMasked { get; set; }
    }
    public partial class Category
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug")]
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

    public partial class Map
    {
        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("coordinates")]
        public Coordinates Coordinates { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }
    }

    public partial class Coordinates
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }
    }

    public partial class Location
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public List<double> Coordinates { get; set; }
    }

    public partial class Properties
    {
        [JsonProperty("CITY")]
        public Category City { get; set; }

        [JsonProperty("TOWN")]
        public Category Town { get; set; }

        [JsonProperty("PRICE")]
        public Price Price { get; set; }

        [JsonProperty("HEATING")]
        public ApartmentFloor Heating { get; set; }

        [JsonProperty("PROJECT")]
        public Project Project { get; set; }

        [JsonProperty("TENANCY")]
        public string Tenancy { get; set; }

        [JsonProperty("CATEGORY")]
        public Category Category { get; set; }

        [JsonProperty("DISTRICT")]
        public Category District { get; set; }

        [JsonProperty("POSITION")]
        public object Position { get; set; }

        [JsonProperty("BED_COUNT")]
        public long BedCount { get; set; }

        [JsonProperty("ROOM_COUNT")]
        public ApartmentFloor RoomCount { get; set; }

        [JsonProperty("STAR_COUNT")]
        public ApartmentFloor StarCount { get; set; }

        [JsonProperty("TRADE_TYPE")]
        public TradeType TradeType { get; set; }

        [JsonProperty("SUBCATEGORY")]
        public Category Subcategory { get; set; }

        [JsonProperty("SWAP_STATUS")]
        public object SwapStatus { get; set; }

        [JsonProperty("BUILDING_AGE")]
        public ApartmentFloor BuildingAge { get; set; }

        [JsonProperty("MUNICIPALITY")]
        public object Municipality { get; set; }

        [JsonProperty("OPEN_AREA_M2")]
        public object OpenAreaM2 { get; set; }

        [JsonProperty("SQUARE_METER")]
        public Price SquareMeter { get; set; }

        [JsonProperty("BUILDING_FEES")]
        public object BuildingFees { get; set; }

        [JsonProperty("BUILDING_TYPE")]
        public ApartmentFloor BuildingType { get; set; }

        [JsonProperty("DEEP_POSITION")]
        public ApartmentFloor DeepPosition { get; set; }

        [JsonProperty("GROUND_SURVEY")]
        public object GroundSurvey { get; set; }

        [JsonProperty("LOADING_GAUGE")]
        public object LoadingGauge { get; set; }

        [JsonProperty("RENTAL_INCOME")]
        public object RentalIncome { get; set; }

        [JsonProperty("TOTAL_AREA_M2")]
        public object TotalAreaM2 { get; set; }

        [JsonProperty("TOULETS_COUNT")]
        public ApartmentFloor TouletsCount { get; set; }

        [JsonProperty("BATHROOM_COUNT")]
        public ApartmentFloor BathroomCount { get; set; }

        [JsonProperty("CLOSED_AREA_M2")]
        public object ClosedAreaM2 { get; set; }

        [JsonProperty("APARTMENT_FLOOR")]
        public ApartmentFloor ApartmentFloor { get; set; }

        [JsonProperty("BUILDING_STATUS")]
        public ApartmentFloor BuildingStatus { get; set; }

        [JsonProperty("CADASTRAL_BLOCK")]
        public object CadastralBlock { get; set; }

        [JsonProperty("LISTING_VARIETY")]
        public object ListingVariety { get; set; }

        [JsonProperty("CADASTRAL_PARCEL")]
        public object CadastralParcel { get; set; }

        [JsonProperty("FLOOR_AREA_RATIO")]
        public object FloorAreaRatio { get; set; }

        [JsonProperty("FURNISHED_OPTION")]
        public ApartmentFloor FurnishedOption { get; set; }

        [JsonProperty("LIVING_ROOM_AREA")]
        public string LivingRoomArea { get; set; }

        [JsonProperty("NET_SQUARE_METER")]
        public string NetSquareMeter { get; set; }

        [JsonProperty("SECURITY_DEPOSIT")]
        public string SecurityDeposit { get; set; }

        [JsonProperty("SHOWCASE_LISTING")]
        public bool ShowcaseListing { get; set; }

        [JsonProperty("RELATED_LOCATIONS")]
        public List<RelatedLocation> RelatedLocations { get; set; }

        [JsonProperty("SETTLEMENT_STATUS")]
        public ApartmentFloor SettlementStatus { get; set; }

        [JsonProperty("SHOWCASE_HOMEPAGE")]
        public bool ShowcaseHomepage { get; set; }

        [JsonProperty("CADASTRAL_MAP_SHEET")]
        public object CadastralMapSheet { get; set; }

        [JsonProperty("FOR_EXCHANGE_STATUS")]
        public object ForExchangeStatus { get; set; }

        [JsonProperty("FOR_TRANSFER_STATUS")]
        public object ForTransferStatus { get; set; }

        [JsonProperty("STRUCTURAL_FEATURES")]
        public object StructuralFeatures { get; set; }

        [JsonProperty("AVAILABLE_FOR_CREDIT")]
        public ApartmentFloor AvailableForCredit { get; set; }

        [JsonProperty("FLOOR_COUNT_OF_BUILDING")]
        public long FloorCountOfBuilding { get; set; }

        [JsonProperty("AVAILABLE_FOR_INVESTMENT")]
        public ApartmentFloor AvailableForInvestment { get; set; }
    }

    public partial class ApartmentFloor
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class Price
    {
        [JsonProperty("value")]
        public long Value { get; set; }
    }

    public partial class Project
    {
        [JsonProperty("id")]

        public long Id { get; set; }
    }

    public partial class RelatedLocation
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class TradeType
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }
    }

    public partial class User
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("account_id")]
        public long AccountId { get; set; }

        [JsonProperty("office_id")]
        public long OfficeId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("category")]
        public long Category { get; set; }

        [JsonProperty("phone")]
        public List<UserPhone> Phone { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("account_name")]
        public string AccountName { get; set; }

        [JsonProperty("logo")]
        public object Logo { get; set; }

        [JsonProperty("logo_full")]
        public object LogoFull { get; set; }

        [JsonProperty("account_product_category")]
        public string AccountProductCategory { get; set; }
    }

    public partial class UserPhone
    {
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("phones")]
        public List<PhonePhone> Phones { get; set; }
    }

    public partial class PhonePhone
    {
        [JsonProperty("did")]
        public string Did { get; set; }

        [JsonProperty("phone_type")]
        public string PhoneType { get; set; }
    }

    
}
