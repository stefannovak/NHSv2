namespace NHSv2.Communications.Application.Services.Contracts;

public interface IEmailService
{
    /// <summary>
    /// Send a plaintext email to the specified destination address.
    /// </summary>
    /// <param name="destinationAddress"></param>
    /// <param name="subject"></param>
    /// <param name="htmlContent"></param>
    /// <returns>Success or failure.</returns>
    public Task<bool> SendEmail(string destinationAddress, string subject, string htmlContent);
}