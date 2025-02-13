using System.Text.Json.Serialization;

namespace NHSv2.Appointments.Application.Models;

public class KeycloakUser
{
    public Guid Id { get; set; }
    
    public string Username { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    // [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;
    
    // [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;
}