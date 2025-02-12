using MediatR;
using NHSv2.Appointments.Application.Dtos.Responses;
using NHSv2.Appointments.Application.Repositories;

namespace NHSv2.Appointments.Application.Appointments.Queries.GetAppointments;

public class GetAppointmentsQueryHandler : IRequestHandler<GetAppointmentsQuery, IList<AppointmentDtoByDoctor>>
{
    private readonly IAppointmentsRepository _appointmentsRepository;

    public GetAppointmentsQueryHandler(IAppointmentsRepository appointmentsRepository)
    {
        _appointmentsRepository = appointmentsRepository;
    }
    
    public async Task<IList<AppointmentDtoByDoctor>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var appointments = _appointmentsRepository
            .GetAppointments(x => x.FacilityName == request.FacilityName && x.DoctorId == request.DoctorId);
        
        var appointmentDtos = appointments.Select(x => new AppointmentDtoByDoctor
        {
            AppointmentId = x.AppointmentId,
            PatientId = x.PatientId,
            Start = x.Start,
            ClientCalendarId = x.ClientCalendarId
        }).ToList();

        return appointmentDtos;
    }
}