using Newtonsoft.Json;
using System.Collections.Generic;

namespace NTools.DTO.MailerSend
{
    public class MailerInfo
    {
        [JsonProperty("from")]
        public MailerRecipientInfo From { get; set; }
        [JsonProperty("to")]
        public IList<MailerRecipientInfo> To { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("html")]
        public string Html { get; set; }
    }
}
