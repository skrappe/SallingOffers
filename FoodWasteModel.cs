using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class FoodWasteOffer
{
    [JsonProperty("clearances")]
    public List<Clearance>? Clearances { get; set; }

    [JsonProperty("store")]
    public Store? Store { get; set; }
}

public class Clearance
{
    [JsonProperty("offer")]
    public Offer? Offer { get; set; }

    [JsonProperty("product")]
    public Product? Product { get; set; }
}

public class Offer
{
    [JsonProperty("originalPrice")]
    public decimal? OriginalPrice { get; set; }

    [JsonProperty("newPrice")]
    public decimal? NewPrice { get; set; }

    [JsonProperty("endTime")]
    public DateTime? EndTime { get; set; }

    [JsonProperty("stock")]
    public double? Stock { get; set; }

    public decimal Savings => (OriginalPrice ?? 0m) - (NewPrice ?? 0m);
}

public class Product
{
    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("categories")]
    public Categories? Categories { get; set; }

    [JsonProperty("image")]
    public string? Image { get; set; }  // Her kan vi vise produktbillede i mail

    public string DanishCategory => Categories?.Da ?? "Ukendt kategori";
}

public class Categories
{
    [JsonProperty("da")]
    public string? Da { get; set; }
}

public class Store
{
    [JsonProperty("name")]
    public string? Name { get; set; }
}
