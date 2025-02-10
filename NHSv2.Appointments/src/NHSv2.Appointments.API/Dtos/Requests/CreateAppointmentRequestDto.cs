namespace NHSv2.Appointments.Dtos.Requests;

/// <summary>
/// Create an appointment request. To be properly validated.
/// </summary>
/// <param name="Start">Start of the appointment.</param>
/// <param name="PatientId">Id of the patient the appointment is for.</param>
/// <param name="Summary">What the appointment is about.</param>
/// <param name="Description">Description of the appointment.</param>
/// <param name="FacilityName">Name of the clinic the appointment is for.</param>
public record CreateAppointmentRequestDto(
    DateTime Start,
    Guid PatientId,
    string Summary,
    string Description,
    string FacilityName);