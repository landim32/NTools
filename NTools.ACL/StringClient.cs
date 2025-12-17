using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NTools.ACL.Interfaces;
using NTools.DTO.Settings;

namespace NTools.ACL
{
    public class StringClient : IStringClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<NToolSetting> _ntoolSetting;
        private readonly ILogger<StringClient> _logger;

        public StringClient(HttpClient httpClient, IOptions<NToolSetting> ntoolSetting, ILogger<StringClient> logger)
        {
            _httpClient = httpClient;
            _ntoolSetting = ntoolSetting;
            _logger = logger;
        }

        public async Task<string> GenerateShortUniqueStringAsync()
        {
            var url = $"{_ntoolSetting.Value.ApiUrl}/String/generateShortUniqueString";
            _logger.LogInformation("Accessing URL: {Url}", url);
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation("Response received: {Response}", result);
            
            return result;
        }

        public async Task<string> GenerateSlugAsync(string name)
        {
            var url = $"{_ntoolSetting.Value.ApiUrl}/String/generateSlug/{name}";
            _logger.LogInformation("Accessing URL: {Url}", url);
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation("Response received: {Response}", result);
            
            return result;
        }

        public async Task<string> OnlyNumbersAsync(string input)
        {
            var url = $"{_ntoolSetting.Value.ApiUrl}/String/onlyNumbers/{input}";
            _logger.LogInformation("Accessing URL: {Url}", url);
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation("Response received: {Response}", result);
            
            return result;
        }
    }
}
