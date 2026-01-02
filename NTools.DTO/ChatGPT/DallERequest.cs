using Newtonsoft.Json;

namespace NTools.DTO.ChatGPT
{
    public class DallERequest
    {
        [JsonProperty("prompt")]
        public string Prompt { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; } = "dall-e-3";

        [JsonProperty("n")]
        public int? NumberOfImages { get; set; } = 1;

        [JsonProperty("size")]
        public string Size { get; set; } = "1024x1024";

        [JsonProperty("quality")]
        public string Quality { get; set; } = "standard";

        [JsonProperty("style")]
        public string Style { get; set; } = "vivid";
    }
}
