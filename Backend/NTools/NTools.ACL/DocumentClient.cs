using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NTools.ACL.Core;
using NTools.ACL.Interfaces;
using NTools.DTO.Domain;
using NTools.DTO.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NTools.ACL
{
    public class DocumentClient: BaseClient, IDocumentClient
    {
        public DocumentClient(IOptions<NToolSetting> ntoolSetting) : base(ntoolSetting)
        {
        }

        public async Task<bool> validarCpfOuCnpjAsync(string cpfCnpj)
        {
            var response = await _httpClient.GetAsync($"{_ntoolSetting.Value.ApiUrl}/Document/validarCpfOuCnpj/{cpfCnpj}");
            response.EnsureSuccessStatusCode();
            return GetBoolFromJson(await response.Content.ReadAsStringAsync());
        }
    }
}
