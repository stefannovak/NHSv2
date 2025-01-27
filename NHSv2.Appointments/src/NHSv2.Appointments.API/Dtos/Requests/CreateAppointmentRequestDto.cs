namespace NHSv2.Appointments.Dtos.Requests;

/// <summary>
/// Create an appointment request.
/// </summary>
/// <param name="Start">Start of the appointment.</param>
/// <param name="End">End of the appointment.</param>
/// <param name="PatientId">Id of the patient the appointment is for.</param>
/// <param name="Description">Description of the appointment.</param>
/// <param name="clinicId">Id of the clinic the appointment is for.</param>
public record CreateAppointmentRequestDto(DateTime Start, DateTime End, Guid PatientId, string Description, Guid clinicId = default);