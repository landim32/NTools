using NTools.DTO.ChatGPT;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTools.ACL.Interfaces
{
    public interface IChatGPTClient
    {
        Task<string> SendMessageAsync(string message);
        Task<string> SendConversationAsync(List<ChatMessage> messages);
        Task<ChatGPTResponse> SendRequestAsync(ChatGPTRequest request);
        Task<DallEResponse> GenerateImageAsync(string prompt);
        Task<DallEResponse> GenerateImageAdvancedAsync(DallERequest request);
    }
}
