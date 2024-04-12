using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using BrainiacsApi.Interface;

namespace BrainiacsApi.Repository
{
    public class SmtpEmailService : IEmailService
    {
    private readonly SmtpClient _smtpClient;

    public SmtpEmailService(SmtpClient smtpClient)
    {
        _smtpClient = smtpClient;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress("brainiacslogics@gmail.com"),
            Subject = subject,
            Body = message,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(email);

        await _smtpClient.SendMailAsync(mailMessage);
    }
}

}