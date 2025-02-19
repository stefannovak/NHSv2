using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NHSv2.Appointments.Application.Configuration;
using NHSv2.Appointments.Application.Helpers.Helpers;
using NHSv2.Appointments.Application.Models;
using NHSv2.Appointments.Application.Services.Contracts;

namespace NHSv2.Appointments.Application.Services;

public class KeycloakService : IKeycloakService
{
    private readonly IOptions<KeycloakOptions> _options;
    private readonly HttpClient _httpClient;

    public KeycloakService(
        IHttpClientFactory httpClientFactory,
        IOptions<KeycloakOptions> options)
    {
        _options = options;
        _httpClient = httpClientFactory.CreateClient("Keycloak");
    }
    
    public async Task<KeycloakUser?> GetUserById(Guid userId)
    {
        using var activity = ActivitySourceHelper.ActivitySource.StartActivity();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/admin/realms/NHSv2-dev/users/{userId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessToken());
        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        
        var content = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<KeycloakUser>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });
        
        return user;
    }
    
    private async Task<string> GetAccessToken()
    {
        var response = await _httpClient.PostAsync("/realms/master/protocol/openid-connect/token", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"client_id", "admin-cli"},
            {"grant_type", "password"},
            {"username", _options.Value.AdminUsername},
            {"password", _options.Value.AdminPassword}
        }));
        
        var content = await response.Content.ReadAsStringAsync();
        var token = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(content)!["access_token"];
        return token.ToString();
    }
}