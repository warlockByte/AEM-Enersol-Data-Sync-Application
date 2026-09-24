using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AemenersolSync.Contracts;

namespace AemenersolSync.Services;

public sealed class PlatformWellApiClient(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Skip
    };

    public async Task<IReadOnlyList<PlatformDto>> GetPlatformsAsync(
        string username, string password, bool useDummy, CancellationToken cancellationToken)
    {
        using var loginResponse = await httpClient.PostAsJsonAsync(
            "api/Account/Login", new { username, password }, JsonOptions, cancellationToken);
        loginResponse.EnsureSuccessStatusCode();
        var token = await loginResponse.Content.ReadFromJsonAsync<string>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("The login response did not contain a bearer token.");
        var endpoint = useDummy ? "api/PlatformWell/GetPlatformWellDummy" : "api/PlatformWell/GetPlatformWellActual";
        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<PlatformDto>>(JsonOptions, cancellationToken) ?? [];
    }
}
