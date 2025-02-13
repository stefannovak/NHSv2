namespace NHSv2.Appointments.Application.Configuration;

public class KeycloakOptions
{
    public const string Keycloak = "Keycloak";
    
    public string AdminUsername { get; set; } = string.Empty;
    
    public string AdminPassword { get; set; } = string.Empty;
}