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

        public DocumentClient(HttpClient httpClient, IOptions<NToolSetting> ntoolSetting)
        {
            _httpClient = httpClient;
            _ntoolSetting = ntoolSetting;
        }

        public async Task<bool> validarCpfOuCnpjAsync(string cpfCnpj)
        {
            var response = await _httpClient.GetAsync($"{_ntoolSetting.Value.ApiUrl}/Document/validarCpfOuCnpj/{cpfCnpj}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<bool>(json);
        }
    }
}
