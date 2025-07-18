using Newtonsoft.Json;


namespace BankSystem.Data.Entities.VirusTotal
{
    public class VirusTotalAnalysis
    {
        [JsonProperty("category")]
        public string? Category { get; set; }

        [JsonProperty("result")]
        public string? Result { get; set; }
    }
}
