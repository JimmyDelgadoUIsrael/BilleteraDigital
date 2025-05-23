using System.Text.Json;

namespace BilleteraDigital.Servicio
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

        public async Task<decimal?> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            try
            {
                string url = $"{BaseUrl}?base={fromCurrency}&symbols={toCurrency}&apikey={ApiKey}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ExchangeRateResponse>(content);

                if (result?.Rates != null && result.Rates.TryGetValue(toCurrency, out var rate))
                {
                    return rate;
                }

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
        public string Base { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
