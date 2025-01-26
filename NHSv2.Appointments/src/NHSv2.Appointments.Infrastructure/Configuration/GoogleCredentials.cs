using System.Reflection;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;

namespace NHSv2.Appointments.Infrastructure.Configuration;

public static class GoogleCredentials
{
    /// <summary>
    /// Gets a Google credential from a Google Service Account JSON file.
    /// See the link below for more information on how to create a Google Service Account.
    /// 
    /// https://developers.google.com/identity/protocols/oauth2/service-account
    /// </summary>
    /// <param name="credentialsPath">
    /// The location of the Google Credentials json file.
    ///
    /// The path should be relative to the root of the project. "Secrets/GoogleCredentials.json" will result in
    /// NHSv2/Secrets/GoogleCredentials.json.
    /// </param>
    /// <returns></returns>
    public static GoogleCredential GetCredential(string credentialsPath)
    {
        var root = Assembly.GetExecutingAssembly().Location.Split("NHSv2.Appointments").First();
        var path = Path.GetFullPath(Path.Combine(root, credentialsPath));
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        return GoogleCredential
            .FromStream(stream)
            .CreateScoped(CalendarService.Scope.Calendar);
    }
}