using Newtonsoft.Json;
namespace BankSystem.Data.Entities.VirusTotal
{
    public class VirusTotalAttributes
    {
        [JsonProperty("last_analysis_results")]
        public Dictionary<string, VirusTotalAnalysis>? LastAnalysisResults { get; set; }

        [JsonProperty("type_description")]
        public string? TypeDescription { get; set; }

        [JsonProperty("size")]
        public long? Size { get; set; }

        [JsonProperty("first_submission_date")]
        public long? FirstSubmissionDate { get; set; }

        [JsonProperty("last_analysis_date")]
        public long? LastAnalysisDate { get; set; }

        [JsonProperty("times_submitted")]
        public int? TimesSubmitted { get; set; }
    }
}
