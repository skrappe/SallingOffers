using System;
using System.Net.Http;
using System.Threading.Tasks;

public class FoodWasteApi
{
    // Indsæt din Salling Group API-nøgle
    private static readonly string apiKey = "SG_APIM_16KW7B8XWYEVWRZ2B4TM7RMNSYSG8G01ZS71J8XG2QSBGS2AGRMG";
    // Eventuelt ændr postnummer
    private static readonly string zipCode = "2800";
    private static readonly string apiUrl = $"https://api.sallinggroup.com/v1/food-waste?zip={zipCode}";

    public static async Task<string> GetFoodWasteDataAsync()
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
