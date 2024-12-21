using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSv2.Communications.Application.Configuration;
using NHSv2.Communications.Application.Services.Contracts;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace NHSv2.Communications.Application.Services;

public class EmailService : IEmailService
{
    private readonly IOptions<SendGridOptions> _sendGridOptions;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<SendGridOptions> sendGridOptions,
        ILogger<EmailService> logger)
    {
        _sendGridOptions = sendGridOptions;
        _logger = logger;

        Client = new SendGridClient(new SendGridClientOptions
        {
            ApiKey = _sendGridOptions.Value.Key,
            HttpErrorAsException = true,
        });
    }

    private SendGridClient Client { get; }

    public async Task SendEmail(string destinationAddress, string subject, string plaintextMessageContent)
    {
        var message = new SendGridMessage
        {
            From = new EmailAddress(_sendGridOptions.Value.FromAddress, "NHSv2"),
        };
        message.AddTo(destinationAddress);
        message.Subject = subject;
        message.PlainTextContent = plaintextMessageContent;
        await TrySendEmail(message);
    }

    private async Task TrySendEmail(SendGridMessage message)
    {
        try
        {
            var response = await Client.SendEmailAsync(message);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Body.ReadAsStringAsync();
                _logger.LogError($"Error sending SendGrid email: {body}");
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to send email. Error: {e.Message}");
        }
    }
}