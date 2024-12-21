using MassTransit;
using NHSv2.Communications.Application.Services.Contracts;
using NHSv2.Messaging.Contracts.MessageContracts;

namespace NHSv2.Communications.Application.Consumers;

public class AppointmentsConsumer : IConsumer<AppointmentCreatedContract>
{
    private readonly IEmailService _emailService;

    public AppointmentsConsumer(IEmailService emailService)
    {
        _emailService = emailService;
    }
    
    public async Task Consume(ConsumeContext<AppointmentCreatedContract> context)
    {
        Console.WriteLine($"Appointment created: {context.Message.AppointmentId}");
        await _emailService.SendEmail("stefannovak96@gmail.com", "Test", context.Message.AppointmentId.ToString());
    }
}