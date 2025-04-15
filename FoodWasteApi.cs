using System;
using System.Net.Http;
using System.Threading.Tasks;

public class FoodWasteApi
{
    // Adjust these as needed (i.e., if you have a dynamic query param for zip):
    private static readonly string zipCode = "2800";
    private static readonly string apiUrl  = $"https://api.sallinggroup.com/v1/food-waste?zip={zipCode}";

    public static async Task<string> GetFoodWasteDataAsync()
    {
        using (HttpClient client = new HttpClient())
        {
            // 1) Retrieve the API key from your SecretsConfig
            var apiKey = SecretsConfig.SallingGroupApiKey;
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Console.WriteLine("Error: SALLINGGROUP_APIKEY (or environment var) is not set.");
                return string.Empty;
            }

            // 2) Set request headers
            client.DefaultRequestHeaders.Add("accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            // 3) Make the GET request
            HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            // 4) Return the raw JSON
            return await response.Content.ReadAsStringAsync();
        }
    }
}
