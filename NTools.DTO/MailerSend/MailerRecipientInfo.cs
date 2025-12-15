using Newtonsoft.Json;

namespace NTools.DTO.MailerSend
{
    public class MailerRecipientInfo
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
