namespace OnlineMovieTicket.BL.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string templateName, Dictionary<string, string> placeholders);
    }
}