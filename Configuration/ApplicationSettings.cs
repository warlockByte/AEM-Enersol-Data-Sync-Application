namespace AemenersolSync.Configuration;

public sealed record ApplicationSettings(string ConnectionString, Uri BaseUrl, string Username, string Password)
{
    public static ApplicationSettings Load(string path)
    {
        using var document = System.Text.Json.JsonDocument.Parse(File.ReadAllText(path));
        var root = document.RootElement;
        var connectionString = Environment.GetEnvironmentVariable("AEM_CONNECTION_STRING")
            ?? root.GetProperty("ConnectionStrings").GetProperty("DefaultConnection").GetString();
        var baseUrl = Environment.GetEnvironmentVariable("AEM_API_BASE_URL")
            ?? root.GetProperty("ApiSettings").GetProperty("BaseUrl").GetString();
        var username = Environment.GetEnvironmentVariable("AEM_API_USERNAME");
        var password = Environment.GetEnvironmentVariable("AEM_API_PASSWORD");
        if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(baseUrl) ||
            string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Set AEM_API_USERNAME and AEM_API_PASSWORD environment variables.");
        return new ApplicationSettings(connectionString, new Uri(baseUrl), username, password);
    }
}
