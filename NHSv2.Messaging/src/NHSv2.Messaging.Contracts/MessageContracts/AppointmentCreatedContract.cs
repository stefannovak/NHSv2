namespace NHSv2.Messaging.Contracts.MessageContracts;

public record AppointmentCreatedContract(Guid AppointmentId, string Subject, string HtmlContent);