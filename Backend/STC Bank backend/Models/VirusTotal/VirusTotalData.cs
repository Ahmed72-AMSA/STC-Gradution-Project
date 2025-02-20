using Newtonsoft.Json;
namespace STC.Models{
public class VirusTotalData
{
    [JsonProperty("attributes")]
    public VirusTotalAttributes? Attributes { get; set; }
}
}