using MediatR;

namespace NHSv2.Appointments.Application.Appointments.Commands.CreateAppointment;

/// <summary>
/// Create an appointment command.
/// </summary>
/// <param name="Start"></param>
/// <param name="End"></param>
/// <param name="PatientId"></param>
/// <param name="Description"></param>
/// <param name="clinicId">To come in v2.</param>
public record CreateAppointmentCommand(
    DateTime Start,
    DateTime End,
    Guid PatientId,
    string Description,
    Guid clinicId = default) : IRequest;