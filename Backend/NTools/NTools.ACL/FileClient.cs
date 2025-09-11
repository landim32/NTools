using Microsoft.AspNetCore.Http;
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
    public class FileClient : BaseClient, IFileClient
    {
        public FileClient(IOptions<NToolSetting> ntoolSetting) : base(ntoolSetting)
        {
        }

        public async Task<string> GetFileUrlAsync(string bucketName, string fileName)
        {
            var response = await _httpClient.GetAsync($"{_ntoolSetting.Value.ApiUrl}/File/{bucketName}/getFileUrl/{fileName}");
            response.EnsureSuccessStatusCode();
            return GetStringFromJson(await response.Content.ReadAsStringAsync());
        }

        public async Task<string> UploadFileAsync(string bucketName, IFormFile file)
        {
            using (var formData = new MultipartFormDataContent())
            {
                using (var fileStream = file.OpenReadStream())
                {
                    var fileContent = new StreamContent(fileStream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    formData.Add(fileContent, "file", file.FileName);
                    var response = await _httpClient.PostAsync($"{_ntoolSetting.Value.ApiUrl}/File/{bucketName}/uploadFile", formData);
                    response.EnsureSuccessStatusCode();

                    return GetStringFromJson(await response.Content.ReadAsStringAsync());
                }
            }
        }
    }
}
