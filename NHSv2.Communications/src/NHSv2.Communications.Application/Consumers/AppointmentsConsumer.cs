using MassTransit;
using NHSv2.Communications.Contracts.MessageContracts;

namespace NHSv2.Communications.Application.Consumers;

public class AppointmentsConsumer : IConsumer<AppointmentCreatedContract>
{
    public async Task Consume(ConsumeContext<AppointmentCreatedContract> context)
    {
        Console.WriteLine($"Appointment created: {context.Message.AppointmentId}");
    }
}