using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainiacsApi.Interface
{
    public interface IEmailService
    {
    Task SendEmailAsync(string email, string subject, string message);
    }

}