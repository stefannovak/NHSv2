using MediatR;
using NHSv2.Appointments.Domain.Appointments.Events;

namespace NHSv2.Appointments.Application.Appointments.Commands.CreateAppointment;

/// <summary>
/// Create an appointment command.
/// </summary>
/// <param name="Start"></param>
/// <param name="PatientEmail"></param>
/// <param name="Summary"></param>
/// <param name="Description"></param>
/// <param name="FacilityName">To come in v2.</param>
/// <param name="DoctorName"></param>
public record CreateAppointmentCommand(
    DateTime Start,
    string PatientEmail,
    string Summary,
    string Description,
    string FacilityName,
    string DoctorName,
    Guid PatientId,
    Guid DoctorId) : IRequest;