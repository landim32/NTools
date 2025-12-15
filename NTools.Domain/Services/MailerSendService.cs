using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NTools.Domain.Services.Interfaces;
using NTools.DTO.MailerSend;
using NTools.DTO.Settings;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NTools.Domain.Services
{
    public class MailerSendService : IMailerSendService
    {
        private readonly IOptions<MailerSendSetting> _mailSettings;

        public MailerSendService(IOptions<MailerSendSetting> mailSettings)
        {
            _mailSettings = mailSettings;
        }

        public async Task<bool> Sendmail(MailerInfo email)
        {
            using (var client = new HttpClient())
            {
                email.From.Email = _mailSettings.Value.MailSender;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mailSettings.Value.ApiToken);
                var jsonContent = new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(_mailSettings.Value.ApiUrl, jsonContent);
                if (!response.IsSuccessStatusCode)
                {
                    var errorStr = await response.Content.ReadAsStringAsync();
                    var msgErro = JsonConvert.DeserializeObject<MailerErrorInfo>(errorStr);
                    if (msgErro != null && !string.IsNullOrEmpty(msgErro.Message))
                    {
                        throw new Exception(msgErro.Message);
                    }
                    throw new Exception("Unknown error");
                }
            }
            return await Task.FromResult(true);
        }
    }
}
