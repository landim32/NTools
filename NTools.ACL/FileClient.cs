using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NTools.ACL.Interfaces;
using NTools.DTO.Settings;

namespace NTools.ACL
{
    public class FileClient : IFileClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<NToolSetting> _ntoolSetting;

        public FileClient(HttpClient httpClient, IOptions<NToolSetting> ntoolSetting)
        {
            _httpClient = httpClient;
            _ntoolSetting = ntoolSetting;
        }

        public async Task<string> GetFileUrlAsync(string bucketName, string fileName)
        {
            var response = await _httpClient.GetAsync($"{_ntoolSetting.Value.ApiUrl}/File/{bucketName}/getFileUrl/{fileName}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<string>(json);
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
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<string>(json);
                }
            }
        }
    }
}
