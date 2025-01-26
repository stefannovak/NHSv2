using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using NHSv2.Appointments.Application.Services.Contracts;
using NHSv2.Appointments.Infrastructure.Configuration;

namespace NHSv2.Appointments.Infrastructure.Services;

public class GoogleCalendarService : ICalendarService
{
    private readonly CalendarService _calendarService;

    private readonly string _calendarId =
        "5692602381dffe8f832e320542e0a37e42c6bdc77c620dd6ecd3eb7e20433f0f@group.calendar.google.com";
    
    public GoogleCalendarService(IConfiguration configuration)
    {
        var credentialsPath = configuration["Google:CredentialsPath"];
        if (credentialsPath is null)
        {
            throw new ArgumentNullException(nameof(credentialsPath), "Google credentials path is required, see GoogleCredentials class documentation.");
        }

        _calendarService = new CalendarService(new BaseClientService.Initializer
        {
            HttpClientInitializer = GoogleCredentials.GetCredential(credentialsPath),
            ApplicationName = "NHSv2"
        });    
    }
    
    public async Task<IList<TimePeriod>> GetAvailableSlotsAsync(DateTime start, DateTime end)
    {
        var request = new FreeBusyRequest
        {
            TimeMinDateTimeOffset = start,
            TimeMaxDateTimeOffset = end,
            Items = new List<FreeBusyRequestItem> { new FreeBusyRequestItem { Id = _calendarId } }
        };

        var response = await _calendarService.Freebusy.Query(request).ExecuteAsync();
        
        // Get busy times
        if (response.Calendars.TryGetValue(_calendarId, out var calendar))
        {
            var busyPeriods = calendar.Busy;
            return busyPeriods;
        }

        return new List<TimePeriod>();
    }
    
    public async Task<Event> CreateAppointmentAsync(string summary, string description, DateTime startTime, string patient)
    {
        var newEvent = new Event
        {
            // https://github.com/stefannovak/NHSv2/issues/1
            // Message[Service accounts cannot invite attendees without Domain-Wide Delegation of Authority.] Location[ - ] Reason[forbiddenForServiceAccounts] Domain[calendar]
            // Attendees = new List<EventAttendee>
            // {
            //     new EventAttendee
            //     {
            //         DisplayName = "Stefan Test",
            //         Email = patient,
            //         Comment = "What a cool dude",
            //     }
            // },
            Summary = summary,
            Description = description,
            Start = new EventDateTime
            {
                DateTimeDateTimeOffset = startTime,
            },
            End = new EventDateTime
            {
                DateTimeDateTimeOffset = startTime.AddMinutes(30),
            },
            Reminders = new Event.RemindersData
            {
                UseDefault = false,
                Overrides = new []
                {
                    new EventReminder { Method = "email", Minutes = 60 * 24 },  // Email reminder 1 Day before
                    new EventReminder { Method = "popup", Minutes = 60 }   // Popup reminder 60 minutes before
                }
            }
        };

        var request = _calendarService.Events.Insert(newEvent, _calendarId);
        return await request.ExecuteAsync();
    }

    public async Task<bool> IsAppointmentSlotAvailable(DateTime start)
    {
        var request = new FreeBusyRequest
        {
            TimeMinDateTimeOffset = start,
            TimeMaxDateTimeOffset = start.AddMinutes(30),
            Items = new List<FreeBusyRequestItem> { new () { Id = _calendarId } }
        };

        var response = await _calendarService.Freebusy.Query(request).ExecuteAsync();
        if (response.Calendars.TryGetValue(_calendarId, out var calendar))
        {
            var busyPeriods = calendar.Busy;
            return busyPeriods.Any() == false;
        }
        
        // TODO: - Proper exception handling.
        throw new Exception("Calendar not found.");
    }
}