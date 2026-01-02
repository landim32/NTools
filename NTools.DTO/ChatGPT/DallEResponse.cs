using Newtonsoft.Json;
using System.Collections.Generic;

namespace NTools.DTO.ChatGPT
{
    public class DallEResponse
    {
        [JsonProperty("created")]
        public long Created { get; set; }

        [JsonProperty("data")]
        public List<DallEImageData> Data { get; set; }
    }
}
