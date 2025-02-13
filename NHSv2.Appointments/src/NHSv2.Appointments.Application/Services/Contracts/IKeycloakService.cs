using NHSv2.Appointments.Application.Models;

namespace NHSv2.Appointments.Application.Services.Contracts;

public interface IKeycloakService
{
    /// <summary>
    /// Get a user by their ID.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<KeycloakUser?> GetUserById(Guid userId);
}