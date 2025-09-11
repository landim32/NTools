using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NTools.ACL.Core;
using NTools.ACL.Interfaces;
using NTools.DTO.MailerSend;
using NTools.DTO.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTools.ACL
{
    public class MailClient : BaseClient, IMailClient
    {
        public MailClient(IOptions<NToolSetting> ntoolSetting) : base(ntoolSetting)
        {
        }

        public async Task<bool> IsValidEmail(string email)
        {
            var response = await _httpClient.GetAsync($"{_ntoolSetting.Value.ApiUrl}/Mail/isValidEmail/{email}");
            response.EnsureSuccessStatusCode();
            return GetBoolFromJson(await response.Content.ReadAsStringAsync());
        }

        public async Task<bool> SendmailAsync(MailerInfo mail)
        {
            var content = new StringContent(JsonConvert.SerializeObject(mail), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_ntoolSetting.Value.ApiUrl}/Mail/sendmail", content);
            response.EnsureSuccessStatusCode();
            return GetBoolFromJson(await response.Content.ReadAsStringAsync());
        }
    }
}
