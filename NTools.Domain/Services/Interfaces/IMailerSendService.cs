using NTools.DTO.MailerSend;
using System.Threading.Tasks;

namespace NTools.Domain.Services.Interfaces
{
    public interface IMailerSendService
    {
        Task<bool> Sendmail(MailerInfo email);
    }
}
