using Newtonsoft.Json;
using System.Collections.Generic;

namespace NTools.DTO.ChatGPT
{
    public class ChatGPTRequest
    {
        [JsonProperty("model")]
        public string Model { get; set; }
        
        [JsonProperty("messages")]
        public List<ChatMessage> Messages { get; set; }
        
        [JsonProperty("temperature")]
        public double Temperature { get; set; } = 0.7;
        
        [JsonProperty("max_completion_tokens")]
        public int? MaxCompletionTokens { get; set; }
    }
}
