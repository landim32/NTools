using Microsoft.Extensions.Logging;
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
        private readonly ILogger<MailClient> _logger;

        public MailClient(HttpClient httpClient, IOptions<NToolSetting> ntoolSetting, ILogger<MailClient> logger)
        {
            _httpClient = httpClient;
            _ntoolSetting = ntoolSetting;
            _logger = logger;
        }

        public async Task<bool> IsValidEmailAsync(string email)
        {
            var url = $"{_ntoolSetting.Value.ApiUrl}/Mail/isValidEmail/{email}";
            _logger.LogInformation("Accessing URL: {Url}", url);
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation("Response received: {Response}", json);
            
            return JsonConvert.DeserializeObject<bool>(json);
        }

        public async Task<bool> SendmailAsync(MailerInfo mail)
        {
            var url = $"{_ntoolSetting.Value.ApiUrl}/Mail/sendmail";
            _logger.LogInformation("Sending email to URL: {Url}", url);
            
            var content = new StringContent(JsonConvert.SerializeObject(mail), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation("Send email response received: {Response}", json);
            
            return JsonConvert.DeserializeObject<bool>(json);
        }
    }
}
