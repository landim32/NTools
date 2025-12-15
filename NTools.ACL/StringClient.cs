using Microsoft.Extensions.Options;
using NTools.ACL.Interfaces;
using NTools.DTO.Settings;

namespace NTools.ACL
{
    public class StringClient : IStringClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<NToolSetting> _ntoolSetting;

        public StringClient(HttpClient httpClient, IOptions<NToolSetting> ntoolSetting)
        {
            _httpClient = httpClient;
            _ntoolSetting = ntoolSetting;
        }

        public async Task<string> GenerateShortUniqueStringAsync()
        {
            var response = await _httpClient.GetAsync($"{_ntoolSetting.Value.ApiUrl}/String/generateShortUniqueString");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GenerateSlugAsync(string name)
        {
            var response = await _httpClient.GetAsync($"{_ntoolSetting.Value.ApiUrl}/String/generateSlug");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> OnlyNumbersAsync(string input)
        {
            var response = await _httpClient.GetAsync($"{_ntoolSetting.Value.ApiUrl}/String/onlyNumbers");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
