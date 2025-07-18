using Newtonsoft.Json;


namespace BankSystem.Data.Entities.VirusTotal
{
    public class VirusTotalData
    {
        [JsonProperty("attributes")]
        public VirusTotalAttributes? Attributes { get; set; }
    }
}
