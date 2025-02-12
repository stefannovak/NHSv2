using MediatR;
using NHSv2.Appointments.Application.Dtos.Responses;

namespace NHSv2.Appointments.Application.Appointments.Queries.GetAppointments;

public class GetAppointmentsQuery : IRequest<IList<AppointmentDtoByDoctor>>
{
    public GetAppointmentsQuery(string facilityName, Guid doctorId)
    {
        FacilityName = facilityName;
        DoctorId = doctorId;
    }
    
    public string FacilityName { get; set; }

    public Guid DoctorId { get; set; }
}