using Newtonsoft.Json;
using System.Collections.Generic;

namespace NTools.DTO.MailerSend
{
    public class MailerErrorInfo
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("errors")]
        public IDictionary<string, IList<string>> Errors { get; set; }
    }
}
