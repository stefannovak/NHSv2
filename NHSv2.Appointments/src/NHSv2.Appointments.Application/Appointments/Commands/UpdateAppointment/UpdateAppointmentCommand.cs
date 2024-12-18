using MediatR;

namespace NHSv2.Appointments.Application.Appointments.Commands.UpdateAppointment;

public record UpdateAppointmentCommand(Guid Id, string Test) : IRequest;