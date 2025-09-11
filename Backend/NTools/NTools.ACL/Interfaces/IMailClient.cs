using NTools.DTO.MailerSend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTools.ACL.Interfaces
{
    public interface IMailClient
    {
        Task<bool> SendmailAsync(MailerInfo mail);
        Task<bool> IsValidEmail(string email);
    }
}
