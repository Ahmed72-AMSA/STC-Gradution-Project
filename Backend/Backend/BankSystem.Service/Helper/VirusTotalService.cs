using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace BankSystem.Service.Helper
{
    public class VirusTotalService
    {
        private readonly string _apiKey;
        private readonly HttpClient _client;

        public VirusTotalService(string apiKey)
        {
            _apiKey = apiKey;
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://www.virustotal.com/api/v3/")
            };
        }

        public async Task<string> CheckFileHashAsync(string hash)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"files/{hash}");
                request.Headers.Add("x-apikey", _apiKey);

                var response = await _client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                }

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonSerializer.Deserialize<JsonElement>(content);
                return json.ToString();  // Return the whole JSON response, or you can extract specific parts.
            }
            catch (Exception ex)
            {
                // Log exception or rethrow as a custom exception if necessary
                return $"Exception: {ex.Message}";
            }
        }
    }
}
