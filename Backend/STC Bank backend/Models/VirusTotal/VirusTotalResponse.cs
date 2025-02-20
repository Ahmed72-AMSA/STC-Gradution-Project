using Newtonsoft.Json;
namespace STC.Models
{
   public class VirusTotalResponse
{
    [JsonProperty("data")]
    public VirusTotalData? Data { get; set; }
} 
}
