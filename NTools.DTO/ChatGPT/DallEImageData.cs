using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTools.DTO.ChatGPT
{
    public class DallEImageData
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("revised_prompt")]
        public string RevisedPrompt { get; set; }
    }
}
