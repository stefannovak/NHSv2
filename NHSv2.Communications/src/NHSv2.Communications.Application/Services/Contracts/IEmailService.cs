namespace NHSv2.Communications.Application.Services.Contracts;

public interface IEmailService
{
    /// <summary>
    /// Send a plaintext email to the specified destination address.
    /// </summary>
    /// <param name="destinationAddress"></param>
    /// <param name="subject"></param>
    /// <param name="plaintextMessageContent"></param>
    /// <returns></returns>
    public Task SendEmail(string destinationAddress, string subject, string plaintextMessageContent);
}