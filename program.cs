using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Program
{
    static async Task Main()
    {
        // 1. HENT DATA
        string jsonData = await FoodWasteApi.GetFoodWasteDataAsync();
        var offers = JsonConvert.DeserializeObject<List<FoodWasteOffer>>(jsonData)
                     ?? new List<FoodWasteOffer>();

        // 2. GRUPPER EFTER BUTIK -> (KATEGORI -> CLEARANCES)
        var storeCategoryOffers = BuildStoreCategoryStructure(offers);

        // 3. FIND DE 5 BEDSTE TILBUD: (storeName, clearance)
        var bestOffers = offers
            .SelectMany(offer => 
                (offer.Clearances ?? new List<Clearance>()).Select(c => 
                    (StoreName: offer.Store?.Name ?? "Ukendt butik", Clearance: c)
                )
            )
            .Where(t => t.Clearance.Offer?.Savings > 0)
            .OrderByDescending(t => t.Clearance.Offer!.Savings)
            .Take(5)
            .ToList();

        // 4. FIND FREMRAGENDE (highlighted) PRODUKTER: (storeName, clearance)
        var importantProducts = new HashSet<string>
        {
            /*"Smør", "mælk", "rugbrød", "emmentaler",
            "hellmans", "haribo", "marabou", "arla",
            "Castus", "neutral", "tulip", "karolines",
            "kims", "pizza", "æg",*/
            "kaffebønner", "mælk", "saftevand", "chips",
            "øl", "sodavand", "vin", "pålæg", 
            "ost", "smør", "chokolade pålæg", 
            "popcorn", "kokos olie",
        };

        var highlightedOffers = offers
            .SelectMany(offer => 
                (offer.Clearances ?? new List<Clearance>()).Select(c => 
                    (StoreName: offer.Store?.Name ?? "Ukendt butik", Clearance: c)
                )
            )
            .Where(t => t.Clearance.Product?.Description != null &&
                        importantProducts.Any(p =>
                            t.Clearance.Product.Description!
                            .Contains(p, StringComparison.OrdinalIgnoreCase))
            )
            .ToList();

        // 5. BYG HTML-EMAIL INDHOLD
        string emailBody = GenerateEmailContent(storeCategoryOffers, bestOffers, highlightedOffers);

        // 6. SEND EMAIL
        await EmailService.SendEmail(
            "skrappe9@hotmail.com",
            "Dagens bedste fødevaretilbud", 
            emailBody
        );

        // 7. PRINT I KONSOLE (STORE->CATEGORY)
        PrintStoreCategoryToConsole(storeCategoryOffers);
    }

    //=== Bygger en 2-niveau ordbog: Store -> (Category -> List of Clearances)
    static Dictionary<string, Dictionary<string, List<Clearance>>> BuildStoreCategoryStructure(
        List<FoodWasteOffer> offers)
    {
        var storeCategory = new Dictionary<string, Dictionary<string, List<Clearance>>>();

        foreach (var offer in offers)
        {
            string storeName = offer.Store?.Name ?? "Ukendt butik";
            if (!storeCategory.ContainsKey(storeName))
            {
                storeCategory[storeName] = new Dictionary<string, List<Clearance>>();
            }

            if (offer.Clearances != null)
            {
                foreach (var clearance in offer.Clearances)
                {
                    string category = clearance.Product?.DanishCategory ?? "Ukendt kategori";
                    if (!storeCategory[storeName].ContainsKey(category))
                    {
                        storeCategory[storeName][category] = new List<Clearance>();
                    }
                    storeCategory[storeName][category].Add(clearance);
                }
            }
        }

        return storeCategory;
    }

    //=== GENERÉR HTML MAIL-INDHOLD
    public static string GenerateEmailContent(
        Dictionary<string, Dictionary<string, List<Clearance>>> storeCategoryOffers,
        List<(string StoreName, Clearance c)> bestOffers,
        List<(string StoreName, Clearance c)> highlightedOffers
    )
    {
        // Start email
        string emailContent = "<h1>Dagens bedste fødevaretilbud</h1>";

        // 1) TOP 5 TILBUD - NU MED STORENAVN
        emailContent += "<h2>Top 5 tilbud (globalt):</h2><ul>";
        foreach (var (storeName, clearance) in bestOffers)
        {
            string desc = clearance.Product?.Description ?? "Ukendt produkt";
            decimal save = clearance.Offer?.Savings ?? 0;
            decimal now  = clearance.Offer?.NewPrice ?? 0;
            emailContent += $"<li><b>{desc}</b> (<i>{storeName}</i>)<br>" +
                            $"{now} DKK (Spar {save} DKK)<br>";

            string? img = clearance.Product?.Image;
            if (!string.IsNullOrEmpty(img))
                emailContent += $"<img src='{img}' width='100' /></li>";
            else
                emailContent += "</li>";
        }
        emailContent += "</ul>";

        // 2) SÆRLIGT INTERESSANTE - NU MED STORENAVN
        if (highlightedOffers.Any())
        {
            emailContent += "<h2>Specielt interessante tilbud:</h2><ul>";
            foreach (var (storeName, clearance) in highlightedOffers)
            {
                string desc = clearance.Product?.Description ?? "Ukendt produkt";
                decimal save = clearance.Offer?.Savings ?? 0;
                decimal now  = clearance.Offer?.NewPrice ?? 0;

                emailContent += $"<li><b>{desc}</b> (<i>{storeName}</i>)<br>" +
                                $"{now} DKK (Spar {save} DKK)<br>";
                string? img = clearance.Product?.Image;
                if (!string.IsNullOrEmpty(img))
                    emailContent += $"<img src='{img}' width='100' /></li>";
                else
                    emailContent += "</li>";
            }
            emailContent += "</ul>";
        }

        // 3) FULDT OVERBLIK (STORE->CATEGORY->ITEMS)
        emailContent += "<h2>Alle tilbud pr. butik og kategori:</h2>";
        // For each store
        foreach (var (storeName, categories) in storeCategoryOffers)
        {
            emailContent += $"<h3>Butik: {storeName}</h3>";
            // For each category in that store
            foreach (var (categoryName, clearanceList) in categories)
            {
                emailContent += $"<h4>Kategori: {categoryName}</h4><ul>";
                decimal totalSavings = 0;
                foreach (var c in clearanceList)
                {
                    string desc = c.Product?.Description ?? "Ukendt produkt";
                    decimal save = c.Offer?.Savings ?? 0;
                    decimal now  = c.Offer?.NewPrice ?? 0;
                    decimal old  = c.Offer?.OriginalPrice ?? 0;
                    double stock = c.Offer?.Stock ?? 0;

                    emailContent += $"<li>{desc}<br>" +
                                    $"Besparelse: {save} DKK<br>" +
                                    $"Pris nu: {now} DKK<br>" +
                                    $"Original pris: {old} DKK<br>" +
                                    $"Antal tilbage: {stock}</li>";
                    totalSavings += save;
                }
                emailContent += $"</ul><b>Samlet besparelse:</b> {totalSavings} DKK<hr>";
            }
        }

        return emailContent;
    }

    //=== UDSKRIVER TIL KONSOLE SAMME STRUKTUR
    static void PrintStoreCategoryToConsole(
        Dictionary<string, Dictionary<string, List<Clearance>>> storeCategoryOffers)
    {
        foreach (var (storeName, categories) in storeCategoryOffers)
        {
            Console.WriteLine($"Butik: {storeName}");
            foreach (var (categoryName, clearanceList) in categories)
            {
                Console.WriteLine($"  Kategori: {categoryName}");
                decimal totalSavings = 0;
                foreach (var c in clearanceList)
                {
                    string desc  = c.Product?.Description ?? "Ukendt produkt";
                    decimal save = c.Offer?.Savings ?? 0;
                    decimal now  = c.Offer?.NewPrice ?? 0;
                    decimal old  = c.Offer?.OriginalPrice ?? 0;
                    double stock = c.Offer?.Stock ?? 0;

                    Console.WriteLine($"    - {desc}");
                    Console.WriteLine($"      Besparelse: {save} DKK");
                    Console.WriteLine($"      Pris nu: {now} DKK");
                    Console.WriteLine($"      Original pris: {old} DKK");
                    Console.WriteLine($"      Antal tilbage: {stock}");
                    totalSavings += save;
                }

                Console.WriteLine($"  Samlet besparelse for denne kategori: {totalSavings} DKK");
                Console.WriteLine(new string('-', 40));
            }
            Console.WriteLine(); // Blank line between stores
        }
    }
}
