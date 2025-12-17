using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NTools.ACL.Interfaces;
using NTools.DTO.Settings;

namespace NTools.ACL
{
    public class DocumentClient : IDocumentClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<NToolSetting> _ntoolSetting;
        private readonly ILogger<DocumentClient> _logger;

        public DocumentClient(HttpClient httpClient, IOptions<NToolSetting> ntoolSetting, ILogger<DocumentClient> logger)
        {
            _httpClient = httpClient;
            _ntoolSetting = ntoolSetting;
            _logger = logger;
        }

        public async Task<bool> validarCpfOuCnpjAsync(string cpfCnpj)
        {
            var url = $"{_ntoolSetting.Value.ApiUrl}/Document/validarCpfOuCnpj/{cpfCnpj}";
            _logger.LogInformation("Accessing URL: {Url}", url);
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation("Response received: {Response}", json);
            
            return JsonConvert.DeserializeObject<bool>(json);
        }
    }
}
