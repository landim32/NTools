using Newtonsoft.Json;

namespace NTools.DTO.ChatGPT
{
    public class ChatGPTErrorResponse
    {
        [JsonProperty("error")]
        public ChatGPTError Error { get; set; }
    }
}
