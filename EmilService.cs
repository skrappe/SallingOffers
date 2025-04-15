using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class EmailService
{
    // Brevo's Transactional Email endpoint
    private const string ApiUrl = "https://api.brevo.com/v3/smtp/email";

    // Sender en mail via Brevo/Sendinblue
    public static async Task SendEmail(string recipientEmail, string subject, string htmlContent)
    {
        var apiKey = SecretsConfig.BrevoApiKey;
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("API key is not set.");
            return;
        }
        using (HttpClient client = new HttpClient())
        {
            // 1) API Key & headers
            client.DefaultRequestHeaders.Add("api-key", apiKey);
            client.DefaultRequestHeaders.Add("accept", "application/json");

            // 2) Build request body
            var emailData = new
            {
                sender = new {
                    name = "Skidegodt tilbud!",
                    email = "your@email.com" // Afsender-mail
                },
                to = new[] {
                    new {
                        email = recipientEmail,
                        name  = "Modtager"
                    }
                },
                subject  = subject,
                htmlContent = htmlContent
            };

            // 3) Serialize
            string json = JsonConvert.SerializeObject(emailData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // 4) POST to /smtp/email
            HttpResponseMessage response = await client.PostAsync(ApiUrl, content);

            // 5) Check the response
            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to send email. Status: {response.StatusCode}\nError: {error}");
            }
            else
            {
                Console.WriteLine("Email sent successfully!");
            }
        }
    }
}
