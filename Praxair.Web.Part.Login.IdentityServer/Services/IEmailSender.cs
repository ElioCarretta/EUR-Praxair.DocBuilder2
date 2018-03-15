using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Praxair.Web.Part.Login.IdentityServer.Services
{
    public interface IEmailSender 
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
