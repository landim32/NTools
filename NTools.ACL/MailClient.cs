using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NTools.ACL.Interfaces;
using NTools.DTO.MailerSend;
using NTools.DTO.Settings;
using System.Text;

namespace NTools.ACL
{
    public class MailClient : IMailClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<NToolSetting> _ntoolSetting;

        public MailClient(HttpClient httpClient, IOptions<NToolSetting> ntoolSetting)
        {
            _httpClient = httpClient;
            _ntoolSetting = ntoolSetting;
        }

        public async Task<bool> IsValidEmailAsync(string email)
        {
            var response = await _httpClient.GetAsync($"{_ntoolSetting.Value.ApiUrl}/Mail/isValidEmail/{email}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<bool>(json);
        }

        public async Task<bool> SendmailAsync(MailerInfo mail)
        {
            var content = new StringContent(JsonConvert.SerializeObject(mail), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_ntoolSetting.Value.ApiUrl}/Mail/sendmail", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<bool>(json);
        }
    }
}
