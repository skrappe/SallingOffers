public static class SecretsConfig
{
    public static string? BrevoApiKey
    {
        get
        {
            // BREVO_API_KEY
            return Environment.GetEnvironmentVariable("BREVO_API_KEY");
        }
    }
    public static string? SallingGroupApiKey
    {
        get
        {
            // SALLING_GROUP_API_KEY 
            return Environment.GetEnvironmentVariable("SALLING_GROUP_API_KEY");
        }
    }
}
