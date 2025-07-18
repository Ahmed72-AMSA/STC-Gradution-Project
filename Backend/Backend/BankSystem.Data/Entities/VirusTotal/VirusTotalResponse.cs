using Newtonsoft.Json;


namespace BankSystem.Data.Entities.VirusTotal
{
    public class VirusTotalResponse
    {
        [JsonProperty("data")]
        public VirusTotalData? Data { get; set; }
    }
}
