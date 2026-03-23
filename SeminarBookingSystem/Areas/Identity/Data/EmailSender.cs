using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress("Seminar Booking System", _configuration["Email:From"]));
        emailMessage.To.Add(MailboxAddress.Parse(email));
        emailMessage.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
        emailMessage.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_configuration["Email:SmtpHost"], int.Parse(_configuration["Email:SmtpPort"]), true);
        await client.AuthenticateAsync(_configuration["Email:Username"], _configuration["Email:Password"]);
        await client.SendAsync(emailMessage);
        await client.DisconnectAsync(true);
    }
}