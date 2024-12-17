using MediatR;

namespace NHSv2.Appointments.Application.Appointments.Commands.CreateAppointment;

/// <summary>
/// Should return the appointment Id.
/// </summary>
/// <param name="Id"></param>
/// <param name="Test"></param>
public record CreateAppointmentCommand(Guid Id, string Test) : IRequest;