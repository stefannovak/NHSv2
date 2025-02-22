using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using NHSv2.Appointments.Application.Dtos.Responses;
using NHSv2.Appointments.Application.Helpers;
using NHSv2.Appointments.Application.Redis;
using NHSv2.Appointments.Application.Repositories;

namespace NHSv2.Appointments.Application.Appointments.Queries.GetAppointments;

public class GetAppointmentsQueryHandler : IRequestHandler<GetAppointmentsQuery, IList<AppointmentDtoByDoctor>>
{
    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly IDistributedCache _cache;
    
    public GetAppointmentsQueryHandler(
        IAppointmentsRepository appointmentsRepository,
        IDistributedCache cache)
    {
        _appointmentsRepository = appointmentsRepository;
        _cache = cache;
    }
    
    public async Task<IList<AppointmentDtoByDoctor>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
    {
        using var activity = ActivitySourceHelper.ActivitySource.StartActivity();
        var cacheKey = CacheKeys.AppointmentsByDoctorAndFacility(request.FacilityName, request.DoctorId);
        var cachedAppointments = await _cache.GetStringAsync(cacheKey, token: cancellationToken);
        if (cachedAppointments != null)
        {
            return JsonSerializer.Deserialize<IList<AppointmentDtoByDoctor>>(cachedAppointments)!;
        }
        
        var appointmentDtos = GetAppointmentsFromDb(request);
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(appointmentDtos), token: cancellationToken);
        
        return GetAppointmentsFromDb(request);;
    }

    private List<AppointmentDtoByDoctor> GetAppointmentsFromDb(GetAppointmentsQuery request)
    {
        using var activity = ActivitySourceHelper.ActivitySource.StartActivity();
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