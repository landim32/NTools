using Microsoft.Extensions.Options;
using NTools.ACL.Core;
using NTools.ACL.Interfaces;
using NTools.DTO.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTools.ACL
{
    public class StringClient : BaseClient, IStringClient
    {
        public StringClient(IOptions<NToolSetting> ntoolSetting) : base(ntoolSetting)
        {
        }

        public async Task<string> GenerateShortUniqueStringAsync()
        {
            var response = await _httpClient.GetAsync($"{_ntoolSetting.Value.ApiUrl}/String/generateShortUniqueString");
            response.EnsureSuccessStatusCode();
            return GetStringFromJson(await response.Content.ReadAsStringAsync());
        }

        public async Task<string> GenerateSlugAsync(string name)
        {
            var response = await _httpClient.GetAsync($"{_ntoolSetting.Value.ApiUrl}/String/generateSlug");
            response.EnsureSuccessStatusCode();
            return GetStringFromJson(await response.Content.ReadAsStringAsync());
        }

        public async Task<string> OnlyNumbersAsync(string input)
        {
            var response = await _httpClient.GetAsync($"{_ntoolSetting.Value.ApiUrl}/String/onlyNumbers");
            response.EnsureSuccessStatusCode();
            return GetStringFromJson(await response.Content.ReadAsStringAsync());
        }
    }
}
