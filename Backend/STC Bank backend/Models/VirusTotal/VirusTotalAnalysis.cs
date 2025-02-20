using Newtonsoft.Json;
namespace STC.Models{
public class VirusTotalAnalysis
{
    [JsonProperty("category")]
    public string? Category { get; set; }
    
    [JsonProperty("result")]
    public string? Result { get; set; }
}
}