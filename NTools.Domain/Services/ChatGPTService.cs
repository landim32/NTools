using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NTools.Domain.Services.Interfaces;
using NTools.DTO.ChatGPT;
using NTools.DTO.Settings;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NTools.Domain.Services
{
    public class ChatGPTService : IChatGPTService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<ChatGPTSetting> _chatGPTSettings;

        public ChatGPTService(HttpClient httpClient, IOptions<ChatGPTSetting> chatGPTSettings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _chatGPTSettings = chatGPTSettings ?? throw new ArgumentNullException(nameof(chatGPTSettings));
        }

        public async Task<string> SendMessageAsync(string message)
        {
            var messages = new List<ChatMessage>
            {
                new ChatMessage
                {
                    Role = "user",
                    Content = message
                }
            };

            return await SendConversationAsync(messages);
        }

        public async Task<string> SendConversationAsync(List<ChatMessage> messages)
        {
            var request = new ChatGPTRequest
            {
                Model = _chatGPTSettings.Value.Model,
                Messages = messages,
                MaxCompletionTokens = 1000
            };

            var response = await SendRequestAsync(request);

            if (response?.Choices != null && response.Choices.Count > 0)
            {
                return response.Choices[0].Message.Content;
            }

            return string.Empty;
        }

        public async Task<ChatGPTResponse> SendRequestAsync(ChatGPTRequest request)
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _chatGPTSettings.Value.ApiKey);

            if (!request.MaxCompletionTokens.HasValue)
            {
                request.MaxCompletionTokens = 1000;
            }

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(request, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }), 
                Encoding.UTF8, 
                "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(
                _chatGPTSettings.Value.ApiUrl, 
                jsonContent);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JsonConvert.DeserializeObject<ChatGPTErrorResponse>(responseContent);
                
                if (errorResponse?.Error != null && !string.IsNullOrEmpty(errorResponse.Error.Message))
                {
                    throw new InvalidOperationException(errorResponse.Error.Message);
                }
                
                throw new InvalidOperationException("Unknown error");
            }

            return JsonConvert.DeserializeObject<ChatGPTResponse>(responseContent);
        }

        public async Task<DallEResponse> GenerateImageAsync(string prompt)
        {
            var request = new DallERequest
            {
                Prompt = prompt,
                Model = "dall-e-3",
                Size = "1024x1024",
                Quality = "standard",
                NumberOfImages = 1
            };

            return await GenerateImageAsync(request);
        }

        public async Task<DallEResponse> GenerateImageAsync(DallERequest request)
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _chatGPTSettings.Value.ApiKey);

            var imageApiUrl = string.IsNullOrEmpty(_chatGPTSettings.Value.ImageApiUrl) 
                ? "https://api.openai.com/v1/images/generations" 
                : _chatGPTSettings.Value.ImageApiUrl;

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(request, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }), 
                Encoding.UTF8, 
                "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(imageApiUrl, jsonContent);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JsonConvert.DeserializeObject<ChatGPTErrorResponse>(responseContent);
                
                if (errorResponse?.Error != null && !string.IsNullOrEmpty(errorResponse.Error.Message))
                {
                    throw new InvalidOperationException(errorResponse.Error.Message);
                }
                
                throw new InvalidOperationException("Unknown error");
            }

            return JsonConvert.DeserializeObject<DallEResponse>(responseContent);
        }
    }
}
