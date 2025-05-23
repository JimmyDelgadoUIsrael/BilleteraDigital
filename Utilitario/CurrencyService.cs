using System.Text.Json;
using System.Text.Json.Serialization;

namespace BilleteraDigital.Utilitario
{
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "fxr_live_a8049daa2b2a0d271bdb5723dc177dc6b95c";
        private const string BaseUrl = "https://api.fxratesapi.com/latest";

        public CurrencyService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<string>> GetAvailableCurrenciesAsync()
        {
            try
            {
                string url = $"{BaseUrl}?Apikey={ApiKey}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return new List<string>();

                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("========== RESPUESTA DE LA API FXRATES ==========");
                Console.WriteLine(content);
                System.Diagnostics.Debug.WriteLine($"Respuesta: {content}");
                Console.WriteLine("=================================================");
                var result = JsonSerializer.Deserialize<ExchangeRateResponse>(content);

                if (result?.Rates != null)
                    return result.Rates.Keys.ToList();

                return new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        public async Task<decimal?> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            try
            {
                string url = $"{BaseUrl}?Apikey={ApiKey}&base={fromCurrency}&symbols={toCurrency}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ExchangeRateResponse>(content);

                if (result?.Rates != null && result.Rates.TryGetValue(toCurrency, out var rate))
                    return rate;

                return null;
            }
            catch
            {
                return null;
            }
        }
    }

    public class ExchangeRateResponse
    {
        [JsonPropertyName("base")]
        public string Base { get; set; }

        [JsonPropertyName("rates")]
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
