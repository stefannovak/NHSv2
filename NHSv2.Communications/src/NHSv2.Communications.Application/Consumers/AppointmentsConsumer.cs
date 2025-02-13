using MassTransit;
using NHSv2.Communications.Application.Services.Contracts;
using NHSv2.Messaging.Contracts.MessageContracts;

namespace NHSv2.Communications.Application.Consumers;

public class AppointmentsConsumer : IConsumer<AppointmentCreatedContract>
{
    private readonly IEmailService _emailService;
    private readonly IBus _bus;

    public AppointmentsConsumer(IEmailService emailService, IBus bus)
    {
        _emailService = emailService;
        _bus = bus;
    }
    
    public async Task Consume(ConsumeContext<AppointmentCreatedContract> context)
    {
        Console.WriteLine($"Appointment created: {context.Message.AppointmentId}");
        var success = await _emailService.SendEmail("stefannovak96+nhsv2@gmail.com", context.Message.Subject, context.Message.HtmlContent);
        if (success)
        {
            await _bus.Publish(new AppointmentEmailSentContract(context.Message.AppointmentId));
        }
    }
}