using MediatR;

namespace NHSv2.Appointments.Application.Appointments.Commands.CreateAppointment;

/// <summary>
/// Create an appointment command.
/// </summary>
/// <param name="Start"></param>
/// <param name="PatientEmail"></param>
/// <param name="Summary"></param>
/// <param name="Description"></param>
/// <param name="facilityName">To come in v2.</param>
public record CreateAppointmentCommand(
    DateTime Start,
    string PatientEmail,
    string Summary,
    string Description,
    string facilityName) : IRequest;