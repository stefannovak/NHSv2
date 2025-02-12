namespace NHSv2.Appointments.Application.Redis;

public static class CacheKeys
{
    public static string AppointmentsByDoctorAndFacility(string facilityName, Guid doctorId) => $"appointments:{facilityName}:{doctorId}";
}