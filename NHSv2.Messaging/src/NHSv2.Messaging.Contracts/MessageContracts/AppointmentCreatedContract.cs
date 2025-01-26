namespace NHSv2.Messaging.Contracts.MessageContracts;

// TODO: - Make this an email specific contract.
public record AppointmentCreatedContract(Guid AppointmentId, string Subject, string HtmlContent);