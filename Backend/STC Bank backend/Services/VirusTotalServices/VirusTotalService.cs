using RestSharp;
using System.Text.Json;
using System.Threading.Tasks;

public class VirusTotalService
{
    private readonly string _apiKey;
    private readonly RestClient _client;

    public VirusTotalService(string apiKey)
    {
        _apiKey = apiKey;
        _client = new RestClient("https://www.virustotal.com/api/v3/");
    }

    public async Task<string> CheckFileHashAsync(string hash)
    {
        var request = new RestRequest($"files/{hash}", Method.Get);
        request.AddHeader("x-apikey", _apiKey);

        var response = await _client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            return $"Error: {response.StatusCode} - {response.ErrorMessage}";
        }

        var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
        return json.ToString();
    }
}
