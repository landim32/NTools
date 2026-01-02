using Newtonsoft.Json;
using System.Collections.Generic;

namespace NTools.DTO.ChatGPT
{
    public class ChatGPTResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("object")]
        public string Object { get; set; }
        
        [JsonProperty("created")]
        public long Created { get; set; }
        
        [JsonProperty("model")]
        public string Model { get; set; }
        
        [JsonProperty("choices")]
        public List<Choice> Choices { get; set; }
        
        [JsonProperty("usage")]
        public Usage Usage { get; set; }
    }
}
