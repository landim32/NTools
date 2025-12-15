using NTools.DTO.MailerSend;

namespace NTools.ACL.Interfaces
{
    public interface IMailClient
    {
        Task<bool> SendmailAsync(MailerInfo mail);
        Task<bool> IsValidEmailAsync(string email);
    }
}
